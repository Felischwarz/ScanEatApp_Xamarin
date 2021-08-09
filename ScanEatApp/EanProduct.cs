using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace ScanEatApp
{
    [Serializable]
    public class EanProduct
    {
        private const string userid = "400000000";

        public string name { get; set; }
        public string detailname { get; set; }
        public string vendor { get; set; }
        public string maincat { get; set; }
        public string subcat { get; set; }
        public int contents { get; set; } //Flags
        public int pack { get; set; } //Flags
        public string descr { get; set; }
        public string origin { get; set; }
        public string validated { get; set; }

        public List<string> content_properties { get; set; }
        public List<string> pack_properties { get; set; }

        private Dictionary<int, string> contents_values = new Dictionary<int, string>
        {
            {1, "laktosefrei"},
            {2, "koffeeinfrei"},
            {4, "diätetisches Lebensmittel"},
            {8, "glutenfrei"},
            {16, "fruktosefrei"},
            {32, "BIO-Lebensmittel nach EU-Ökoverordnung"},
            {64, "fair gehandeltes Produkt nach FAIRTRADE™-Standard"},
            {128, "vegetarisch"},
            {256, "vegan"},
            {512, "Warnung vor Mikroplastik"},
            {1024, "Warnung vor Mineralöl"},
            {2048, "Warnung vor Nikotin"},
        };

        private Dictionary<int, string> pack_values = new Dictionary<int, string>
        {
            {1, "die Verpackung besteht überwiegend aus Plastik"},
            {2, "die Verpackung besteht überwiegend aus Verbundmaterial"},
            {4, "die Verpackung besteht überwiegend aus Papier/Pappe"},
            {8, "die Verpackung besteht überwiegend aus Glas/Keramik/Ton"},
            {16, "die Verpackung besteht überwiegend aus Metall"},
            {32, "ist unverpackt"},
            {64, "die Verpackung ist komplett frei von Plastik"},
            {128, "Artikel ist übertrieben stark verpackt"},
            {256, "Artikel ist angemessen sparsam verpackt"},
            {512, "Pfandsystem / Mehrwegverpackung"},
        };


        public DateTime timeStamp { get; private set; }

        public EanProduct(string EAN)
        {
            WebRequest request = WebRequest.Create("http://opengtindb.org/?ean=" + EAN + "&cmd=query&queryid=" + userid);
            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //Checking for network errors
            if (response.StatusDescription != "OK")
            {
                Console.WriteLine("Response code: " + response.StatusDescription + ". Stopped creating a product...");
                return;
            }

            Stream dataStream = response.GetResponseStream();
            var encoding = response.CharacterSet == "" ? Encoding.UTF8 : Encoding.GetEncoding(response.CharacterSet);
            StreamReader reader = new StreamReader(dataStream, encoding);

            string responseText = reader.ReadToEnd();
            string[] responseLines = responseText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            reader.Close();
            dataStream.Close();
            response.Close();

            //Checking for EAN errors
            string[] lineValues = responseLines[1].Split('=');
            if(lineValues.Length < 2)
            {
                Console.WriteLine("No Ean errorcode found! Stopped creating a product...");
                return;
            }
            bool success = Int32.TryParse(lineValues[1], out int EanErrorCode);
            if (!success) 
            {
                Console.WriteLine("No Ean errorcode found! Stopped creating a product...");
                return;
            }

            if(EanErrorCode != 0)
            {
                Console.WriteLine("EAN ERROR: " + EanErrorCode.ToString() + ". Stopped creating a product...");
                return;
            }

            //Creating EanProduct
            timeStamp = DateTime.Now;

            foreach(string line in responseLines)
            {
                string[] fields = line.Split('=');

                //skip if there is no field
                if(fields.Length < 2) { continue; }          
                else if(fields[1] == String.Empty) { continue; } 
                    
                switch (fields[0])
                {
                    case nameof(name):
                        name = fields[1];
                        break;

                    case nameof(detailname):
                        detailname = fields[1];
                        break;

                    case nameof(vendor):
                        vendor = fields[1];
                        break;

                    case nameof(maincat):
                        maincat = fields[1];
                        break;

                    case nameof(subcat):
                        subcat = fields[1];
                        break;

                    case nameof(contents):
                        Int32.TryParse(fields[1], out int _contents);
                        contents = _contents;
                        break;

                    case nameof(pack):
                        Int32.TryParse(fields[1], out int _pack);
                        pack = _pack;
                        break;
                      
                    case nameof(descr):
                        descr = fields[1];
                        break;

                    case nameof(origin):
                        origin = fields[1];
                        break;

                    case nameof(validated):
                        validated = fields[1];
                        break;
                }
            }

            content_properties = new List<string>();
            pack_properties = new List<string>();
            getContentProperties();
            getPackProperties();
        }

        public EanProduct(string manualProductName, bool isManualProduct)
        {
            if(!isManualProduct)
            {
                Console.WriteLine("ERROR: Passed not manual Product to wrong overload!");
            }

            name = manualProductName;
            timeStamp = DateTime.Now;
        }

        //test product
        public EanProduct()
        {
            name = "test product";
            detailname = "details";
            vendor = "vendor";
            maincat = "maincat";
            subcat = "subcat";
            contents = 0;
            pack = 0;
            descr = "descr";
            origin = "origin";
            validated = "validated";

            timeStamp = DateTime.Now;
        }

        public void getContentProperties()
        {
            if(contents == null)
            {
                return;
            }

            List<int> flags = getFlags(contents);
            foreach(int flag in flags)
            {
                content_properties.Add(contents_values[flag]);
            }
        }

        public void getPackProperties()
        {
            if(pack == null)
            {
                return;
            }

            List<int> flags = getFlags(pack);
            foreach (int flag in flags)
            {
                if(flag == 32)
                {
                    //ist unverpackt (immer zusammen mit 64, nie zusammen mit 128 und 256)
                    bool found_64 = false;   
                    foreach (int flag2 in flags)
                    {
                        if(flag2 == 128 || flag2 == 256)
                        {
                            flags.Remove(flag2);
                        }

                        if(flag2 == 64)
                        {
                            found_64 = true;
                        }
                    }

                    if(!found_64)
                    {
                        pack_properties.Add(pack_values[64]);
                    }
                    
                }

                if (flag == 64)
                {
                    //die Verpackung ist komplett frei von Plastik (nie zusammen mit 1 oder 2)
                    foreach (int flag2 in flags)
                    {
                        if (flag2 == 1 || flag2 == 2)
                        {
                            flags.Remove(flag2);
                        }
                    }
                }

                if (flag == 128)
                {
                    //Artikel ist übertrieben stark verpackt (nie zusammen mit 32)
                    foreach (int flag2 in flags)
                    {
                        if (flag2 == 32)
                        {
                            flags.Remove(flag2);
                        }
                    }
                }

                pack_properties.Add(pack_values[flag]);
            }
        }

        public List<int> getFlags(int value)
        {
            List<int> Flags = new List<int>();

            while(value > 0)
            {
                int res;
                if (value == 1)
                {
                    res = 1;
                }
                else
                {
                    int exp = 0;
                    res = 0;
                    while ((int)Math.Pow(2, exp) < value)
                    {
                        res = (int)Math.Pow(2, exp);
                        exp++;
                    }                    
                }
                Flags.Add(res);
                value -= res;
            }
            return Flags;
        }

        public void LogData()
        {
            Console.WriteLine(
                "name: " + name + "\n" +
                "detailname: " + detailname + "\n" +
                "vendor: " + vendor + "\n" +
                "maincat: " + maincat + "\n" +
                "subcat: " + subcat + "\n" +
                "contents: " + contents.ToString() + "\n" +
                "pack: " + pack.ToString() + "\n" +
                "descr: " + descr + "\n" +
                "origin: " + origin + "\n" +
                "validated: " + validated + "\n" + 
                "timeStemp: " + timeStamp + "\n"
                );
        }   
    }
}
