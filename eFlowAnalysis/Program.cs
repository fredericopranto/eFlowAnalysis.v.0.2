using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace eFlowAnalysis
{
    public class eFlow
    {
        public string fluxo { get; set; }
        public int specialized { get; set; }
        public int uncaught { get; set; }
        public int uncaughtspecialized { get; set; }
        public int uncaughtsubsumption { get; set; }
        public int subsumption { get; set; }
        public string sequencial { get; set; }
        public string tipoexcecao { get; set; }
        public string tipo { get; set; }

        public eFlow() { }

        public eFlow(string sequencial, string fluxo, string tipo)
        {
            this.fluxo = fluxo;

            if (tipo.Trim().Equals("specialized"))
                this.specialized++;
            if (tipo.Trim().Equals("uncaught"))
                this.uncaught++;
            if (tipo.Trim().Equals("uncaughtspecialized"))
                this.uncaughtspecialized++;
            if (tipo.Trim().Equals("uncaughtsubsumption"))
                this.uncaughtsubsumption++;
            if (tipo.Trim().Equals("subsumption"))
                this.subsumption++;

            this.specialized = specialized;
            this.uncaught = uncaught;
            this.uncaughtspecialized = uncaughtspecialized;
            this.uncaughtsubsumption = uncaughtsubsumption;
            this.subsumption = subsumption;
            this.sequencial = sequencial;
            this.tipo = tipo;
        }


        public eFlow(string sequencial, string tipoexcecao, string fluxo, string tipo)
        {
            this.tipoexcecao = tipoexcecao;
            this.fluxo = fluxo;

            if (tipo.Trim().Equals("specialized"))
                this.specialized++;
            if (tipo.Trim().Equals("uncaught"))
                this.uncaught++;
            if (tipo.Trim().Equals("uncaughtspecialized"))
                this.uncaughtspecialized++;
            if (tipo.Trim().Equals("uncaughtsubsumption"))
                this.uncaughtsubsumption++;
            if (tipo.Trim().Equals("subsumption"))
                this.subsumption++;

            this.specialized = specialized;
            this.uncaught = uncaught;
            this.uncaughtspecialized = uncaughtspecialized;
            this.uncaughtsubsumption = uncaughtsubsumption;
            this.subsumption = subsumption;
            this.sequencial = sequencial;
            this.tipo = tipo;
        }

        public eFlow(string sequencial, string fluxo, int uncaught, int uncaughtspecialized, int uncaughtsubsumption, int specialized, int subsumption)
        {
            this.fluxo = fluxo;
            this.specialized = specialized;
            this.uncaught = uncaught;
            this.uncaughtspecialized = uncaughtspecialized;
            this.uncaughtsubsumption = uncaughtsubsumption;
            this.subsumption = subsumption;
            this.sequencial = sequencial;
        }

        public eFlow(string sequencial, string tipoexcecao, string fluxo, int uncaught, int uncaughtspecialized, int uncaughtsubsumption, int specialized, int subsumption)
        {
            this.tipoexcecao = tipoexcecao;
            this.fluxo = fluxo;
            this.specialized = specialized;
            this.uncaught = uncaught;
            this.uncaughtspecialized = uncaughtspecialized;
            this.uncaughtsubsumption = uncaughtsubsumption;
            this.subsumption = subsumption;
            this.sequencial = sequencial;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {

            Console.Write("Digite o path: ");
            var path = @"C:\Users\fredericopranto\Downloads\Analisado";

            Console.WriteLine("Lendo arquivos para analisar...");
            string[] filePaths = Directory.GetFiles(path, "*.csv", SearchOption.AllDirectories);
            var directory = new FileInfo(filePaths[0]).DirectoryName;
            filePaths = filePaths.Where(w => !w.Contains("[analisado]")).ToArray();
            filePaths = filePaths.Where(w => !w.Contains("[analisado_par]")).ToArray();

            //foreach (var filePath in filePaths)
            //    Parallel.ForEach(filePaths, currentFile =>
            //    { });

            foreach (var filePath in filePaths)
            {
                Dictionary<string, eFlow> fluxosAnalizados = AnalisysCSVFile(filePath);
                Dictionary<string, eFlow> fluxosAnalizadosPorTipo = AnalisysCSVFilePorTipo(filePath);
                WriteAnalizedCSVFile((Dictionary<string, eFlow>)fluxosAnalizados, filePath.Replace(".csv", "[analisado].csv"));
                WriteAnalizedCSVFilePorTipo((Dictionary<string, eFlow>)fluxosAnalizadosPorTipo, filePath.Replace(".csv", "[analisadoPorTipo].csv"));
            }

            Console.WriteLine("Lendo arquivos para comparar...");
            string[] filePathsCompare = Directory.GetFiles(path, "*.csv", SearchOption.AllDirectories);
            string[] filePathsComparePorTipo = Directory.GetFiles(path, "*.csv", SearchOption.AllDirectories);
            filePathsCompare = filePathsCompare.Where(w => w.Contains("[analisado]")).ToArray();
            filePathsComparePorTipo = filePathsComparePorTipo.Where(w => w.Contains("[analisadoPorTipo]")).ToArray();

            for (int i = 0; i < filePathsCompare.Length - 1; i++)
            {
                var fluxosAnalizados = CompareCSVFile(filePathsCompare[i], filePathsCompare[i + 1]);
                WriteAnalizedCSVFile2(
                    (List<eFlow>)fluxosAnalizados,
                        directory + @"\" +
                        "[" + new FileInfo(filePathsCompare[i]).Name.Substring(0, new FileInfo(filePathsCompare[i]).Name.IndexOf('[')) + "]" +
                        "[" + new FileInfo(filePathsCompare[i + 1]).Name.Substring(0, new FileInfo(filePathsCompare[i + 1]).Name.IndexOf('[')) + "]" +
                        "[analisado_par].csv");
            }

            for (int i = 0; i < filePathsComparePorTipo.Length - 1; i++)
            {
                var fluxosAnalizados = CompareCSVFilePorTipo(filePathsComparePorTipo[i], filePathsComparePorTipo[i + 1]);
                WriteAnalizedCSVFile2(
                    (List<eFlow>)fluxosAnalizados,
                        directory + @"\" +
                        "[" + new FileInfo(filePathsComparePorTipo[i]).Name.Substring(0, new FileInfo(filePathsComparePorTipo[i]).Name.IndexOf('[')) + "]" +
                        "[" + new FileInfo(filePathsComparePorTipo[i + 1]).Name.Substring(0, new FileInfo(filePathsComparePorTipo[i + 1]).Name.IndexOf('[')) + "]" +
                        "[analisadoPorTipo_par].csv");
            }

            Console.WriteLine("-------------- OK ----------------");
        }

        private static List<PropertyInfo> GetSelectedProperties(PropertyInfo[] props, string include, string exclude)
        {
            List<PropertyInfo> propList = new List<PropertyInfo>();
            if (include != "") //Do include first
            {
                var includeProps = include.ToLower().Split(',').ToList();
                foreach (var item in props)
                {
                    var propName = includeProps.Where(a => a == item.Name.ToLower()).FirstOrDefault();
                    if (!string.IsNullOrEmpty(propName))
                        propList.Add(item);
                }
            }
            else if (exclude != "") //Then do exclude
            {
                var excludeProps = exclude.ToLower().Split(',');
                foreach (var item in props)
                {
                    var propName = excludeProps.Where(a => a == item.Name.ToLower()).FirstOrDefault();
                    if (string.IsNullOrEmpty(propName))
                        propList.Add(item);
                }
            }
            else //Default
            {
                propList.AddRange(props.ToList());
            }
            return propList;
        }


        private static string GetSimpleTypeName<T>(IList<T> list)
        {
            string typeName = list.GetType().ToString();
            int pos = typeName.IndexOf("[") + 1;
            typeName = typeName.Substring(pos, typeName.LastIndexOf("]") - pos);
            typeName = typeName.Substring(typeName.LastIndexOf(".") + 1);
            return typeName;
        }

        public static string CreateCsvFile<T>(IList<T> list, string path, string include, string exclude)
        {
            //Variables for build CSV string
            StringBuilder sb = new StringBuilder();
            List<string> propNames;
            List<string> propValues;
            bool isNameDone = false;

            //Get property collection and set selected property list
            PropertyInfo[] props = typeof(T).GetProperties();
            List<PropertyInfo> propList = GetSelectedProperties(props, include, exclude);

            //Add list name and total count
            string typeName = GetSimpleTypeName(list);
            sb.AppendLine(string.Format("{0} List - Total Count: {1}", typeName, list.Count.ToString()));

            //Iterate through data list collection
            foreach (var item in list)
            {
                sb.AppendLine("");
                propNames = new List<string>();
                propValues = new List<string>();

                //Iterate through property collection
                foreach (var prop in propList)
                {
                    //Construct property name string if not done in sb
                    if (!isNameDone) propNames.Add(prop.Name);

                    //Construct property value string with double quotes for issue of any comma in string type data
                    var val = prop.PropertyType == typeof(string) ? "\"{0}\"" : "{0}";
                    propValues.Add(string.Format(val, prop.GetValue(item, null)));
                }
                //Add line for Names
                string line = string.Empty;
                if (!isNameDone)
                {
                    line = string.Join(",", propNames);
                    sb.AppendLine(line);
                    isNameDone = true;
                }
                //Add line for the values
                line = string.Join(",", propValues);
                sb.Append(line);
            }
            if (!string.IsNullOrEmpty(sb.ToString()) && path != "")
            {
                File.WriteAllText(path, sb.ToString());
            }
            return path;
        }


        private static Dictionary<string, eFlow> AnalisysCSVFile(string filePath)
        {

            Console.WriteLine("Analisando arquivo" + filePath);

            List<eFlow> fluxos = new List<eFlow>();
            Dictionary<string, eFlow> fluxosAnalizados = new Dictionary<string, eFlow>();

            //1
            foreach (var line in File.ReadLines(filePath))
            {
                var values = line.Split(';');
                if (!values[0].Trim('"').Trim().Equals("Sequencial"))
                    fluxos.Add(new eFlow(values[0].Trim('"'), values[6].Trim('"'), values[5].Trim('"')));
            }


            //2
            //List<string> lines = File.ReadLines(filePath).ToList();
            //int contador = lines.Count; //2613

            //long size;
            //size = new FileInfo(filePath).Length;
            //GC.AddMemoryPressure(size);


            //for (int i = 0; i < 500; i++)
            //{
            //    var values = lines[i].Split(';');
            //    if (!values[0].Trim('"').Equals("Sequencial"))
            //        fluxos.Add(new eFlow(values[0].Trim('"'), values[6].Trim('"'), values[5].Trim('"')));
            //}


            //for (int i = 500; i < 1000; i++)
            //{
            //    var values = lines[i].Split(';');
            //    if (!values[0].Trim('"').Equals("Sequencial"))
            //        fluxos2.Add(new eFlow(values[0].Trim('"'), values[6].Trim('"'), values[5].Trim('"')));
            //}

            //for (int i = 1000; i < contador; i++)
            //{
            //    var values = lines[i].Split(';');
            //    if (!values[0].Trim('"').Equals("Sequencial"))
            //        fluxos3.Add(new eFlow(values[0].Trim('"'), values[6].Trim('"'), values[5].Trim('"')));
            //}


            //3
            //List<string> lines = File.ReadLines(filePath).ToList();
            //Parallel.ForEach(lines, currentFile =>
            //{
            //    var values = currentFile.Split(';');
            //    if (!values[0].Trim('"').Equals("Sequencial"))
            //        fluxos.Add(new eFlow(values[0].Trim('"'), values[6].Trim('"'), values[5].Trim('"')));    

            //} //close lambda expression
            //     ); //close method invocation



            //4
            //using (StreamReader readerFile = new StreamReader(filePath))
            //{
            //    string line;
            //    while ((line = readerFile.ReadLine()) != null)
            //    {
            //        var values = line.Split(';');
            //        if (!values[0].Trim('"').Equals("Sequencial"))
            //            fluxos.Add(new eFlow(values[0].Trim('"'), values[6].Trim('"'), values[5].Trim('"')));
            //    }

            //    //while (!readerFile.EndOfStream)
            //    //{
            //    //    var values = readerFile.ReadLine().Split(';');
            //    //    if (!values[0].Trim('"').Equals("Sequencial"))
            //    //        fluxos.Add(new eFlow(values[0].Trim('"'), values[6].Trim('"'), values[5].Trim('"')));
            //    //}
            //}

            //5
            //using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //using (BufferedStream bs = new BufferedStream(fs))
            //using (StreamReader sr = new StreamReader(bs))
            //{
            //    string line;
            //    int i;
            //    while ((line = sr.ReadLine()) != null)
            //    {
            //        var values = line.Split(';');
            //        var opa1 = values[0].Trim('"');
            //        var opa2 = values[6].Trim('"');
            //        var opa3 = values[5].Trim('"');
            //        eFlow eFlow = new eFlow(opa1, opa2, opa3);

            //        if (!values[0].Trim('"').Equals("Sequencial"))
            //            fluxos.Add(eFlow);
            //    }
            //}


            string[] stringSeparators = new string[] { "<-" };
            foreach (var pair in fluxos)
            {
                string[] split = pair.fluxo.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < split.Length; i++)
                {
                    if (split[i].Contains(')'))
                        split[i] = split[i].Substring(split[i].IndexOf('<') + 1, (split[i].IndexOf('>') - split[i].IndexOf('<')) - 1);

                    if (fluxosAnalizados.ContainsKey((split[i])))
                    {
                        continue;
                    }

                    foreach (var pair2 in fluxos)
                    {
                        if (pair2.fluxo.Contains(split[i]))
                        {
                            if (!fluxosAnalizados.ContainsKey(split[i]))
                            {
                                if (split[i].Trim() != "\"")
                                    if (split[i].Length > 5)
                                        fluxosAnalizados.Add(split[i], new eFlow(pair2.sequencial, split[i], pair2.tipo.Trim()));
                            }
                            else
                            {
                                if (pair2.tipo.Trim().Equals("specialized"))
                                    fluxosAnalizados[split[i]].specialized++;
                                if (pair2.tipo.Trim().Equals("uncaught"))
                                    fluxosAnalizados[split[i]].uncaught++;
                                if (pair2.tipo.Trim().Equals("uncaughtspecialized"))
                                    fluxosAnalizados[split[i]].uncaughtspecialized++;
                                if (pair2.tipo.Trim().Equals("uncaughtsubsumption"))
                                    fluxosAnalizados[split[i]].uncaughtsubsumption++;
                                if (pair2.tipo.Trim().Equals("subsumption"))
                                    fluxosAnalizados[split[i]].subsumption++;
                            }
                        }
                    }
                }
            }
            return fluxosAnalizados;
        }

        private static Dictionary<string, eFlow> AnalisysCSVFilePorTipo(string filePath)
        {

            Console.WriteLine("Analisando arquivo por tipo" + filePath);

            List<eFlow> fluxos = new List<eFlow>();
            Dictionary<string, eFlow> fluxosAnalizados = new Dictionary<string, eFlow>();

            //1
            foreach (var line in File.ReadLines(filePath))
            {
                var values = line.Split(';');
                if (!values[0].Trim('"').Trim().Equals("Sequencial"))
                    fluxos.Add(new eFlow(values[0].Trim('"'), values[3].Trim('"'), values[6].Trim('"'), values[5].Trim('"')));
            }


            //2
            //List<string> lines = File.ReadLines(filePath).ToList();
            //int contador = lines.Count; //2613

            //long size;
            //size = new FileInfo(filePath).Length;
            //GC.AddMemoryPressure(size);


            //for (int i = 0; i < 500; i++)
            //{
            //    var values = lines[i].Split(';');
            //    if (!values[0].Trim('"').Equals("Sequencial"))
            //        fluxos.Add(new eFlow(values[0].Trim('"'), values[6].Trim('"'), values[5].Trim('"')));
            //}


            //for (int i = 500; i < 1000; i++)
            //{
            //    var values = lines[i].Split(';');
            //    if (!values[0].Trim('"').Equals("Sequencial"))
            //        fluxos2.Add(new eFlow(values[0].Trim('"'), values[6].Trim('"'), values[5].Trim('"')));
            //}

            //for (int i = 1000; i < contador; i++)
            //{
            //    var values = lines[i].Split(';');
            //    if (!values[0].Trim('"').Equals("Sequencial"))
            //        fluxos3.Add(new eFlow(values[0].Trim('"'), values[6].Trim('"'), values[5].Trim('"')));
            //}


            //3
            //List<string> lines = File.ReadLines(filePath).ToList();
            //Parallel.ForEach(lines, currentFile =>
            //{
            //    var values = currentFile.Split(';');
            //    if (!values[0].Trim('"').Equals("Sequencial"))
            //        fluxos.Add(new eFlow(values[0].Trim('"'), values[6].Trim('"'), values[5].Trim('"')));    

            //} //close lambda expression
            //     ); //close method invocation



            //4
            //using (StreamReader readerFile = new StreamReader(filePath))
            //{
            //    string line;
            //    while ((line = readerFile.ReadLine()) != null)
            //    {
            //        var values = line.Split(';');
            //        if (!values[0].Trim('"').Equals("Sequencial"))
            //            fluxos.Add(new eFlow(values[0].Trim('"'), values[6].Trim('"'), values[5].Trim('"')));
            //    }

            //    //while (!readerFile.EndOfStream)
            //    //{
            //    //    var values = readerFile.ReadLine().Split(';');
            //    //    if (!values[0].Trim('"').Equals("Sequencial"))
            //    //        fluxos.Add(new eFlow(values[0].Trim('"'), values[6].Trim('"'), values[5].Trim('"')));
            //    //}
            //}

            //5
            //using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //using (BufferedStream bs = new BufferedStream(fs))
            //using (StreamReader sr = new StreamReader(bs))
            //{
            //    string line;
            //    int i;
            //    while ((line = sr.ReadLine()) != null)
            //    {
            //        var values = line.Split(';');
            //        var opa1 = values[0].Trim('"');
            //        var opa2 = values[6].Trim('"');
            //        var opa3 = values[5].Trim('"');
            //        eFlow eFlow = new eFlow(opa1, opa2, opa3);

            //        if (!values[0].Trim('"').Equals("Sequencial"))
            //            fluxos.Add(eFlow);
            //    }
            //}


            string[] stringSeparators = new string[] { "<-" };
            foreach (var pair in fluxos)
            {
                string[] split = pair.fluxo.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < split.Length; i++)
                {
                    if (split[i].Contains(')'))
                        split[i] = split[i].Substring(split[i].IndexOf('<') + 1, (split[i].IndexOf('>') - split[i].IndexOf('<')) - 1);

                    if (fluxosAnalizados.ContainsKey(split[i] + "|" + pair.tipoexcecao))
                    {
                        //// Adicionado o condicional para continuar o processo caso o novo fluxo seja de um tipo de exceção diferente do que ja está cadastrado
                        //if (((eFlow)fluxosAnalizados[split[i]]).tipoexcecao.Equals(pair.tipoexcecao))
                        continue;
                    }

                    foreach (var pair2 in fluxos)
                    {
                        if (pair2.fluxo.Contains(split[i]))
                        {

                            if (!fluxosAnalizados.ContainsKey(split[i] + "|" + pair2.tipoexcecao))
                            {
                                if (split[i].Trim() != "\"")
                                    if (split[i].Length > 5)
                                        fluxosAnalizados.Add(split[i] + "|" + pair2.tipoexcecao, new eFlow(pair2.sequencial, pair2.tipoexcecao, split[i], pair2.tipo.Trim()));
                            }
                            //else if (fluxosAnalizados.ContainsKey(split[i]) && !((eFlow)fluxosAnalizados[split[i]]).tipoexcecao.Equals(pair.tipoexcecao))// Verificação retomada para adicionar o fluxo com tipo diferente
                            //{
                            //    if (split[i].Trim() != "\"")
                            //        if (split[i].Length > 5)
                            //            fluxosAnalizados.Add(split[i], new eFlow(pair2.sequencial, pair2.tipoexcecao, split[i], pair2.tipo.Trim()));
                            //}
                            else
                            {
                                if (pair2.tipo.Trim().Equals("specialized"))
                                    fluxosAnalizados[split[i] + "|" + pair2.tipoexcecao].specialized++;
                                if (pair2.tipo.Trim().Equals("uncaught"))
                                    fluxosAnalizados[split[i] + "|" + pair2.tipoexcecao].uncaught++;
                                if (pair2.tipo.Trim().Equals("uncaughtspecialized"))
                                    fluxosAnalizados[split[i] + "|" + pair2.tipoexcecao].uncaughtspecialized++;
                                if (pair2.tipo.Trim().Equals("uncaughtsubsumption"))
                                    fluxosAnalizados[split[i] + "|" + pair2.tipoexcecao].uncaughtsubsumption++;
                                if (pair2.tipo.Trim().Equals("subsumption"))
                                    fluxosAnalizados[split[i] + "|" + pair2.tipoexcecao].subsumption++;
                            }
                        }
                    }
                }
            }
            return fluxosAnalizados;
        }

        private static List<eFlow> CompareCSVFile(string filePath1, string filePath2)
        {
            List<eFlow> fluxos = new List<eFlow>();
            List<eFlow> fluxos1 = new List<eFlow>();
            List<eFlow> fluxos2 = new List<eFlow>();
            List<eFlow> fluxosAnalizados = new List<eFlow>();

            var readerFile1 = new StreamReader(
                    File.OpenRead(filePath1));

            var readerFile2 = new StreamReader(
                    File.OpenRead(filePath2));

            Console.WriteLine("Analisando arquivo 1" + filePath1);

            while (!readerFile1.EndOfStream)
            {
                var values = readerFile1.ReadLine().Split(';');
                if (!values[0].Trim().Equals("sequencial"))
                    fluxos1.Add(new eFlow(values[0], values[1], int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4]), int.Parse(values[5]), int.Parse(values[6])));
            }

            Console.WriteLine("Analisando arquivo 2" + filePath2);

            while (!readerFile2.EndOfStream)
            {
                var values = readerFile2.ReadLine().Split(';');
                if (!values[0].Trim().Equals("sequencial"))
                    fluxos2.Add(new eFlow(values[0], values[1], int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4]), int.Parse(values[5]), int.Parse(values[6])));
            }

            Console.WriteLine("Comparando arquivos");

            fluxos = fluxos1.Concat(fluxos2).ToList();
            IEnumerable<IGrouping<string, eFlow>> opa = fluxos
                .GroupBy(i => i.fluxo);

            //eFlow eFlowDefaultIfEmpty = new eFlow();
            //IEnumerable opa2 = opa.Select(g => g.Aggregate<eFlow>((current, next) => new eFlow
            //    {
            //        uncaught = current.uncaught - next.uncaught,
            //        specialized = current.specialized - next.specialized,
            //        subsumption = current.subsumption - next.subsumption,
            //        fluxo = current.fluxo,
            //        sequencial = current.sequencial
            //    }));

            //var total = from f1 in fluxos1
            //            join f2 in fluxos2 on f1.fluxo equals f2.fluxo
            //            group new { f1, f2 } by f1.fluxo into f
            //            select 
            //            new 
            //                {
            //                    uncaught = f.Key.uncaught - f.Sum(x => x.f2.uncaught),
            //                    specialized = f.Key. current.specialized - next.specialized,
            //                    subsumption = current.subsumption - next.subsumption,
            //                    fluxo = current.fluxo,
            //                    sequencial = current.sequencial
            //                };


            //var mergedList = fluxos1.Concat(fluxos2)
            //          .GroupBy(f => f.fluxo)
            //          .Select(group => group.DefaultIfEmpty(new eFlow()).Aggregate(
            //                             (current, next) => new eFlow
            //    {
            //        uncaught = next.uncaught - current.uncaught,
            //        specialized = next.specialized - current.specialized,
            //        subsumption = next.subsumption - current.subsumption,
            //        fluxo = next.fluxo,
            //        sequencial = next.sequencial
            //    } )).ToList();


            var leftOuterJoin = from first in fluxos1
                                join last in fluxos2
                                on first.fluxo equals last.fluxo
                                into g
                                from last in g.DefaultIfEmpty(new eFlow())
                                select new eFlow
                                {
                                    uncaught = last.uncaught - first.uncaught,
                                    uncaughtspecialized = last.uncaughtspecialized - first.uncaughtspecialized,
                                    uncaughtsubsumption = last.uncaughtsubsumption - first.uncaughtsubsumption,
                                    specialized = last.specialized - first.specialized,
                                    subsumption = last.subsumption - first.subsumption,
                                    fluxo = first.fluxo,
                                    sequencial = first.sequencial
                                };
            var rightOuterJoin = from last in fluxos2
                                 join first in fluxos1
                                 on last.fluxo equals first.fluxo
                                 into g
                                 from first in g.DefaultIfEmpty(new eFlow())
                                 select new eFlow
                                 {
                                     uncaught = last.uncaught - first.uncaught,
                                     uncaughtspecialized = last.uncaughtspecialized - first.uncaughtspecialized,
                                     uncaughtsubsumption = last.uncaughtsubsumption - first.uncaughtsubsumption,
                                     specialized = last.specialized - first.specialized,
                                     subsumption = last.subsumption - first.subsumption,
                                     fluxo = last.fluxo,
                                     sequencial = last.sequencial
                                 };
            var fullOuterJoin = leftOuterJoin.Union(rightOuterJoin).Distinct();

            foreach (eFlow item in fullOuterJoin.ToList().GroupBy(x => x.fluxo).Select(y => y.First()))
            {
                fluxosAnalizados.Add(item);
            }

            return fluxosAnalizados;
        }

        private static List<eFlow> CompareCSVFilePorTipo(string filePath1, string filePath2)
        {
            List<eFlow> fluxos = new List<eFlow>();
            List<eFlow> fluxos1 = new List<eFlow>();
            List<eFlow> fluxos2 = new List<eFlow>();
            List<eFlow> fluxosAnalizados = new List<eFlow>();

            var readerFile1 = new StreamReader(
                    File.OpenRead(filePath1));

            var readerFile2 = new StreamReader(
                    File.OpenRead(filePath2));

            Console.WriteLine("Analisando arquivo 1" + filePath1);

            while (!readerFile1.EndOfStream)
            {
                var values = readerFile1.ReadLine().Split(';');
                if (!values[0].Trim().Equals("sequencial"))
                    fluxos1.Add(new eFlow(values[0], values[1], values[2], int.Parse(values[3]), int.Parse(values[4]), int.Parse(values[5]), int.Parse(values[6]), int.Parse(values[7])));
            }

            Console.WriteLine("Analisando arquivo 2" + filePath2);

            while (!readerFile2.EndOfStream)
            {
                var values = readerFile2.ReadLine().Split(';');
                if (!values[0].Trim().Equals("sequencial"))
                    fluxos2.Add(new eFlow(values[0], values[1], values[2], int.Parse(values[3]), int.Parse(values[4]), int.Parse(values[5]), int.Parse(values[6]), int.Parse(values[7])));
            }

            Console.WriteLine("Comparando arquivos");

            fluxos = fluxos1.Concat(fluxos2).ToList();
            IEnumerable<IGrouping<string, eFlow>> opa = fluxos
                .GroupBy(i => i.fluxo);

            //eFlow eFlowDefaultIfEmpty = new eFlow();
            //IEnumerable opa2 = opa.Select(g => g.Aggregate<eFlow>((current, next) => new eFlow
            //    {
            //        uncaught = current.uncaught - next.uncaught,
            //        specialized = current.specialized - next.specialized,
            //        subsumption = current.subsumption - next.subsumption,
            //        fluxo = current.fluxo,
            //        sequencial = current.sequencial
            //    }));

            //var total = from f1 in fluxos1
            //            join f2 in fluxos2 on f1.fluxo equals f2.fluxo
            //            group new { f1, f2 } by f1.fluxo into f
            //            select 
            //            new 
            //                {
            //                    uncaught = f.Key.uncaught - f.Sum(x => x.f2.uncaught),
            //                    specialized = f.Key. current.specialized - next.specialized,
            //                    subsumption = current.subsumption - next.subsumption,
            //                    fluxo = current.fluxo,
            //                    sequencial = current.sequencial
            //                };


            //var mergedList = fluxos1.Concat(fluxos2)
            //          .GroupBy(f => f.fluxo)
            //          .Select(group => group.DefaultIfEmpty(new eFlow()).Aggregate(
            //                             (current, next) => new eFlow
            //    {
            //        uncaught = next.uncaught - current.uncaught,
            //        specialized = next.specialized - current.specialized,
            //        subsumption = next.subsumption - current.subsumption,
            //        fluxo = next.fluxo,
            //        sequencial = next.sequencial
            //    } )).ToList();


            var leftOuterJoin = from first in fluxos1
                                join last in fluxos2
                                on string.Concat(first.fluxo, "|", first.tipoexcecao) equals string.Concat(last.fluxo, "|", last.tipoexcecao)
                                into g
                                from last in g.DefaultIfEmpty(new eFlow())
                                select new eFlow
                                {
                                    tipoexcecao = first.tipoexcecao,
                                    uncaught = last.uncaught - first.uncaught,
                                    uncaughtspecialized = last.uncaughtspecialized - first.uncaughtspecialized,
                                    uncaughtsubsumption = last.uncaughtsubsumption - first.uncaughtsubsumption,
                                    specialized = last.specialized - first.specialized,
                                    subsumption = last.subsumption - first.subsumption,
                                    fluxo = first.fluxo,
                                    sequencial = first.sequencial
                                };
            var rightOuterJoin = from last in fluxos2
                                 join first in fluxos1
                                 on string.Concat(last.fluxo, "|", last.tipoexcecao) equals string.Concat(first.fluxo, "|", first.tipoexcecao)
                                 into g
                                 from first in g.DefaultIfEmpty(new eFlow())
                                 select new eFlow
                                 {
                                     tipoexcecao = last.tipoexcecao,
                                     uncaught = last.uncaught - first.uncaught,
                                     uncaughtspecialized = last.uncaughtspecialized - first.uncaughtspecialized,
                                     uncaughtsubsumption = last.uncaughtsubsumption - first.uncaughtsubsumption,
                                     specialized = last.specialized - first.specialized,
                                     subsumption = last.subsumption - first.subsumption,
                                     fluxo = last.fluxo,
                                     sequencial = last.sequencial
                                 };
            var fullOuterJoin = leftOuterJoin.Union(rightOuterJoin).Distinct();

            foreach (eFlow item in fullOuterJoin.ToList().GroupBy(x => string.Concat(x.fluxo, "|", x.tipoexcecao)).Select(y => y.First()))
            {
                fluxosAnalizados.Add(item);
            }

            return fluxosAnalizados;
        }

        public static void WriteAnalizedCSVFile<T>(IEnumerable<T> items, string path)
        {

            Console.WriteLine("Escrevendo arquivo analisado: " + path);

            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(p => p.Name);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(";", "sequencial", "fluxo", "uncaught", "uncaughtspecialized", "uncaughtsubsumption", "specialized", "subsumption"));

                foreach (var item in items)
                {
                    KeyValuePair<string, eFlow> newItem = (KeyValuePair<string, eFlow>)(object)item;

                    writer.Write(newItem.Value.sequencial);
                    writer.Write(";");
                    writer.Write(newItem.Value.fluxo.Replace("\"\"", "\""));
                    writer.Write(";");
                    writer.Write(newItem.Value.uncaught);
                    writer.Write(";");
                    writer.Write(newItem.Value.uncaughtspecialized);
                    writer.Write(";");
                    writer.Write(newItem.Value.uncaughtsubsumption);
                    writer.Write(";");
                    writer.Write(newItem.Value.specialized);
                    writer.Write(";");
                    writer.Write(newItem.Value.subsumption);
                    writer.Write("\n");
                }
            }
        }

        public static void WriteAnalizedCSVFilePorTipo<T>(IEnumerable<T> items, string path)
        {

            Console.WriteLine("Escrevendo arquivo analisado por tipo: " + path);

            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(p => p.Name);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(";", "sequencial", "tipoexcecao", "fluxo", "uncaught", "uncaughtspecialized", "uncaughtsubsumption", "specialized", "subsumption"));

                foreach (var item in items)
                {
                    KeyValuePair<string, eFlow> newItem = (KeyValuePair<string, eFlow>)(object)item;

                    writer.Write(newItem.Value.sequencial);
                    writer.Write(";");
                    writer.Write(newItem.Value.tipoexcecao);
                    writer.Write(";");
                    writer.Write(newItem.Value.fluxo.Replace("\"\"", "\""));
                    writer.Write(";");
                    writer.Write(newItem.Value.uncaught);
                    writer.Write(";");
                    writer.Write(newItem.Value.uncaughtspecialized);
                    writer.Write(";");
                    writer.Write(newItem.Value.uncaughtsubsumption);
                    writer.Write(";");
                    writer.Write(newItem.Value.specialized);
                    writer.Write(";");
                    writer.Write(newItem.Value.subsumption);
                    writer.Write("\n");
                }
            }
        }

        public static void WriteAnalizedCSVFile2<T>(IEnumerable<T> items, string path)
        {

            Console.WriteLine("Escrevendo arquivo comparado: " + path);

            Type itemType = typeof(T);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(";", "sequencial", "tipoexcecao", "fluxo", "uncaught", "uncaughtspecialized", "uncaughtsubsumption", "specialized", "subsumption"));

                foreach (var item in items)
                {
                    eFlow newItem = (eFlow)(object)item;

                    writer.Write(newItem.sequencial);
                    writer.Write(";");
                    writer.Write(newItem.tipoexcecao);
                    writer.Write(";");
                    writer.Write(newItem.fluxo.Replace("\"\"", "\""));
                    writer.Write(";");
                    writer.Write(newItem.uncaught);
                    writer.Write(";");
                    writer.Write(newItem.uncaughtspecialized);
                    writer.Write(";");
                    writer.Write(newItem.uncaughtsubsumption);
                    writer.Write(";");
                    writer.Write(newItem.specialized);
                    writer.Write(";");
                    writer.Write(newItem.subsumption);
                    writer.Write("\n");
                }
            }
        }
    }
}