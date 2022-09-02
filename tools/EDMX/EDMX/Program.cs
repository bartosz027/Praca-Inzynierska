using System;
using System.IO;
using System.Linq;

namespace EDMX {

    class Program {
        static void Main(string[] args) {
            CreateEDMXFile(args[0]);
            ModifyNavigationProperties(args[0], args[1]);
        }


        static void CreateEDMXFile(string edmx_dir) {
            var edmx_file = File.ReadAllText(edmx_dir + "PiDbModel.edmx");

            // SSDL file
            var ssdl_file = File.ReadAllLines(edmx_dir + "PiDbModel.ssdl").ToList();

            ssdl_file.RemoveRange(0, 2);
            ssdl_file.RemoveAt(ssdl_file.Count - 1);

            ssdl_file[0] = ssdl_file[0].Substring(2);

            for (int i = 1; i < ssdl_file.Count; i++) {
                ssdl_file[i] = ssdl_file[i].Insert(0, new string(' ', 6));
            }

            // CSDL file
            var csdl_file = File.ReadAllLines(edmx_dir + "PiDbModel.csdl").ToList();

            csdl_file.RemoveRange(0, 2);
            csdl_file.RemoveAt(csdl_file.Count - 1);

            csdl_file[0] = csdl_file[0].Substring(2);

            for (int i = 1; i < csdl_file.Count; i++) {
                csdl_file[i] = csdl_file[i].Insert(0, new string(' ', 6));
            }

            // MSL file
            var msl_file = File.ReadAllLines(edmx_dir + "PiDbModel.msl").ToList();

            msl_file.RemoveRange(0, 2);
            msl_file.RemoveAt(msl_file.Count - 1);

            msl_file[0] = msl_file[0].Substring(2);

            for (int i = 1; i < msl_file.Count; i++) {
                msl_file[i] = msl_file[i].Insert(0, new string(' ', 6));
            }

            // Replace placeholders
            var ssdl_result = String.Join(Environment.NewLine, ssdl_file);
            edmx_file = edmx_file.Replace("$SSDL$", ssdl_result);

            var csdl_result = String.Join(Environment.NewLine, csdl_file);
            edmx_file = edmx_file.Replace("$CSDL$", csdl_result);

            var msl_result = String.Join(Environment.NewLine, msl_file);
            edmx_file = edmx_file.Replace("$MSL$", msl_result);

            // Override EDMX
            File.WriteAllText(edmx_dir + "PiDbModel.edmx", edmx_file);
        }

        static void ModifyNavigationProperties(string edmx_dir, string script_filepath) {
            var edmx_file = File.ReadAllLines(edmx_dir + "PiDbModel.edmx");
            var script_file = File.ReadAllLines(script_filepath);

            var current_table = "";
            var current_property = "";

            foreach (var line in script_file) {
                if(line.Contains("CREATE TABLE")) {
                    var data = line.Split(' ').ElementAt(2);
                    
                    if(!data.Contains('(')) {
                        current_table = data; 
                    }
                    else {
                        current_table = data.Substring(0, data.Length - 1);
                    }
                }
                else if(line.Contains("-- NP:")) {
                    var data = line.Split(' ');

                    for(int i = 0; i < data.Length; i++) {
                        if(data[i].Contains("NP:")) {
                            current_property = data[i].Substring(3);
                        }
                        
                        if(data[i].Contains("rename_to")) {
                            var start_index = data[i].IndexOf('\"');
                            var end_index = data[i].LastIndexOf('\"');

                            var value =  data[i].Substring(start_index + 1, end_index - start_index - 1);
                            RenameNavigationProperty(ref edmx_file, current_table, current_property, value);
                        }

                        if(data[i].Contains("remove_property")) {
                            RemoveNavigationProperty(ref edmx_file, current_table, current_property);
                        }
                    }
                }
            }

            File.WriteAllLines(edmx_dir + "PiDbModel.edmx", edmx_file);
        }


        private static void RenameNavigationProperty(ref string[] edmx, string current_table, string current_property, string value) {
            bool csdl_content = false;
            bool entity_type = false;

            for (int i = 0; i < edmx.Length; i++) {
                if (edmx[i].Contains("<!-- CSDL content -->")) {
                    csdl_content = true;
                    entity_type = false;
                }

                if (edmx[i].Contains("<EntityType Name=\"" + current_table + "\">")) {
                    entity_type = true;
                }

                if ((csdl_content && entity_type) && edmx[i].Contains("<NavigationProperty Name=\"" + current_property + "\"")) {
                    var curr_property_name = "Name=\"" + current_property + "\"";
                    var new_property_name = "Name=\"" + value + "\"";

                    edmx[i] = edmx[i].Replace(curr_property_name, new_property_name);
                    break;
                }
            }
        }

        private static void RemoveNavigationProperty(ref string[] edmx, string current_table, string current_property) {
            bool csdl_content = false;
            bool entity_type = false;

            for (int i = 0; i < edmx.Length; i++) {
                if (edmx[i].Contains("<!-- CSDL content -->")) {
                    csdl_content = true;
                    entity_type = false;
                }

                if (edmx[i].Contains("<EntityType Name=\"" + current_table + "\">")) {
                    entity_type = true;
                }

                if ((csdl_content && entity_type) && edmx[i].Contains("<NavigationProperty Name=\"" + current_property + "\"")) {
                    var line_to_be_removed = edmx[i];
                    edmx = edmx.Where(p => p != line_to_be_removed).ToArray();

                    break;
                }
            }
        }
    }

}