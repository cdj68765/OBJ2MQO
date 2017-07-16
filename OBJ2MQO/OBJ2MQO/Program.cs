using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Text.RegularExpressions;

namespace OBJ2MQO
{
    class Program
    {
        public class UV
        {
            public float U;
            public float V;

            public UV(float u, float v,string n)
            {
                U = u;
                V = v;
            }
        }

        private static void Main()
        {
            // var ConObjList = new ConcurrentDictionary<int, List<UV>>();
            var ConObjList = new Dictionary<string, List<UV>>();
            int Show = 0;
            int ShowFin = 0;
            var Mission = new Task(() =>
            {
                var ObjList =
                    new DirectoryInfo(@"C:\Users\cdj68\Desktop\07-15\Hop Step Sing!\HSS_PV1_Data\AS3\Mesh")
                        .GetFiles("*.obj");
                ShowFin = ObjList.Length;
                //Parallel.ForEach(ObjList, temp =>
                foreach (var temp in ObjList)
                {
                    int VertexCount = 0;
                    int FaceCount = 0;
                    var floatlist = new List<UV>();
                    using (TextReader Read = new StreamReader(new FileStream(temp.FullName, FileMode.Open),
                        Encoding.Default))
                    {
                        while (Read.Peek() != -1)
                        {
                            if (VertexCount == 7119)
                            {
                                Console.WriteLine();
                            }
                            var Linedata = Read.ReadLine();
                            if (Linedata.StartsWith("vt"))
                            {
                                var FloatString = Linedata.Split(' ');
                                floatlist.Add(new UV(float.Parse(FloatString[1]), float.Parse(FloatString[2]), temp.Name));
                            }
                            else if (Linedata.StartsWith("v"))
                            {
                                Interlocked.Increment(ref VertexCount);
                            }
                            else if (Linedata.StartsWith("f"))
                            {
                                Interlocked.Increment(ref FaceCount);
                            }
                        }
                        //ConObjList.TryAdd(VertexCount, floatlist);
                        ConObjList.Add(temp.Name, floatlist);
                        Interlocked.Increment(ref Show);
                    }
                }
                //);
            });
            var Fin = false;
            Mission.ContinueWith(x =>
            {
                using (var File = new FileStream(@"C:\Users\cdj68\Desktop\Model\Model.mqo", FileMode.Open))
                {
                    ShowFin = (int) File.Length;
                    using (TextReader ReadMqo = new StreamReader(File, Encoding.Default))
                    {
                        StringBuilder BuildMqo = new StringBuilder();
                        var TempUV = new List<UV>();
                        while (ReadMqo.Peek() != -1)
                        {
                            var Linedata = ReadMqo.ReadLine();
                            Show = ReadMqo.Peek();
                            if (Linedata.StartsWith("\tvertex"))
                            {
                              /*  if (!ConObjList.TryGetValue(int.Parse(Linedata.Split(' ')[1]), out TempUV))
                                {
                                    Console.WriteLine();
                                }*/
                            }
                            else if (Linedata.StartsWith("\tfacet"))
                            {
                                BuildMqo.AppendLine(Linedata);
                                continue;
                            }
                            else if (Linedata.StartsWith("\tface"))
                            {
                                BuildMqo.AppendLine(Linedata);
                                var FaceCount = int.Parse(Linedata.Split(' ')[1]);
                                for (int i = 0; i < FaceCount; i++)
                                {
                                    Linedata = ReadMqo.ReadLine();
                                    Secend.Matches(FirstReg.Match(Linedata).Groups[2].Value);
                                    foreach (var mc in from Match p in Secend.Matches(FirstReg.Match(Linedata).Groups[2]
                                                           .Value)
                                                       select Vertex.Matches(p.Groups["val"].Value))
                                    {
                                        StringBuilder SaveFaceUV = new StringBuilder();
                                        SaveFaceUV.Append(TempUV[int.Parse(mc[0].Value)].U);
                                        SaveFaceUV.Append(" ");
                                        SaveFaceUV.Append(TempUV[int.Parse(mc[0].Value)].V);
                                        SaveFaceUV.Append(" ");
                                        SaveFaceUV.Append(TempUV[int.Parse(mc[1].Value)].U);
                                        SaveFaceUV.Append(" ");
                                        SaveFaceUV.Append(TempUV[int.Parse(mc[1].Value)].V);
                                        SaveFaceUV.Append(" ");
                                        SaveFaceUV.Append(TempUV[int.Parse(mc[2].Value)].U);
                                        SaveFaceUV.Append(" ");
                                        SaveFaceUV.Append(TempUV[int.Parse(mc[2].Value)].V);
                                        Linedata = Linedata.Replace("0 1 0 1 0 1", SaveFaceUV.ToString());
                                        break;
                                    }
                                    BuildMqo.AppendLine(Linedata);
                                }
                            }
                            BuildMqo.AppendLine(Linedata);
                        }
                        using (StreamWriter sw =
                            new StreamWriter(@"C:\Users\cdj68\Desktop\Model\Model2.mqo", false, Encoding.Default))
                        {
                            sw.Write(BuildMqo.ToString());
                            Fin = true;
                        }
                    }
                }
            });
            //Mission.Start();
            new Task(() =>
            {
                var ObjList =
                    new DirectoryInfo(@"C:\Users\cdj68\Desktop\07-15\Hop Step Sing!\HSS_PV1_Data\AS3\Mesh")
                        .GetFiles("*.obj");
                ShowFin = ObjList.Length;
                foreach (var temp in ObjList)
                {
                    var name = temp.Name.Replace(".obj", "").Replace(" ","");
                    using (TextReader Read = new StreamReader(new FileStream(temp.FullName, FileMode.Open),
                        Encoding.Default))
                    {
                        StringBuilder BuildMqo = new StringBuilder();
                        var Count=0;
                        var Last = "";
                        while (Read.Peek() != -1)
                        {
                            var Linedata = Read.ReadLine();
                            var Now = "";
                            if (Linedata.StartsWith("g"))
                            {
                                Linedata = "g " + name;
                                Now = "g";
                            }
                            else if (Linedata.StartsWith("usemtl"))
                            {
                                Linedata = "usemtl " + name;

                            }
                            else if (Linedata.StartsWith("f"))
                            {
                               var s1= Linedata.Split(' ');
                                StringBuilder face = new StringBuilder("f ");
                                var Num= int.Parse(s1[1].Split('/')[0]);
                                face.Append(Num);
                                face.Append('/');
                                face.Append(Num);
                                face.Append('/');
                                face.Append(Num);
                                face.Append(' ');
                                Num = int.Parse(s1[2].Split('/')[0]);
                                face.Append(Num);
                                face.Append('/');
                                face.Append(Num);
                                face.Append('/');
                                face.Append(Num);
                                face.Append(' ');
                                Num = int.Parse(s1[3].Split('/')[0]);
                                face.Append(Num);
                                face.Append('/');
                                face.Append(Num);
                                face.Append('/');
                                face.Append(Num);
                                Linedata = face.ToString();
                                Interlocked.Increment(ref Count);
                                Now = "f";
                            }
                            else if (Linedata.StartsWith("vt"))
                            {
                                Interlocked.Increment(ref Count);
                                Now = "vt";
                            }
                            else if (Linedata.StartsWith("vn"))
                            {
                                Interlocked.Increment(ref Count);
                                Now = "vn";
                            }
                            else if (Linedata.StartsWith("v"))
                            {
                                Interlocked.Increment(ref Count);
                                Now = "v";
                            }

                            if (Last != Now)
                            {
                                switch (Last)
                                {
                                    case "v":
                                        BuildMqo.AppendLine("# " + Count + " vertices");
                                        break;
                                    case "vt":
                                        BuildMqo.AppendLine("# " + Count + " texture vertices");
                                        break;
                                    case "vn":
                                        BuildMqo.AppendLine("# " + Count + " normal vertices");
                                        break;
                                }
                                Last = Now;
                                if (Last =="v"|| Last=="vt"||Last=="vn")
                                {
                                    BuildMqo.AppendLine();
                                }
                                Count = 0;
                              
                            }
                            BuildMqo.AppendLine(Linedata);
                        }
                        BuildMqo.AppendLine("# " + Count + " elements");
                        Read.Close();
                        using (StreamWriter sw = new StreamWriter(temp.FullName, false, Encoding.Default))
                        {
                            sw.Write(BuildMqo.ToString());
                        }
                    }
                    using (TextReader Read = new StreamReader(new FileStream(temp.FullName.Replace(".obj", ".mtl"), FileMode.Open),
                        Encoding.Default))
                    {
                        StringBuilder BuildMqo = new StringBuilder();
                        bool Find = true;
                        while (Read.Peek() != -1)
                        {
                            var Linedata = Read.ReadLine();
                            if (Find)
                            {
                                if (Linedata.StartsWith("newmtl"))
                                {
                                    Linedata = "newmtl " + name;
                                    Find = false;

                                }
                            }
                            BuildMqo.AppendLine(Linedata);
                        }
                        Read.Close();
                        using (StreamWriter sw = new StreamWriter(temp.FullName.Replace(".obj", ".mtl"), false, Encoding.Default))
                        {
                            sw.Write(BuildMqo.ToString());
                        }
                    }
                    Interlocked.Increment(ref Show);
                }
                Fin = true;
            }).Start();
            while (!Fin)
            {
                Console.WriteLine(Show.ToString() + @"/" + ShowFin.ToString());
                Console.WriteLine(Show.ToString() + @"/" + ShowFin.ToString());
            }

        }


        private static readonly Regex FirstReg = new Regex(@"([234]) (.+)$", RegexOptions.Compiled);

        private static readonly Regex Secend = new Regex(@"(?<key>\w+)\((?:""(?<val>.*)""|(?<val>[^\)]+))\)",
            RegexOptions.Compiled);

        private static readonly Regex Vertex = new Regex(@"(-?\d+(?:\.\d+)?)", RegexOptions.Compiled);
    }
}
