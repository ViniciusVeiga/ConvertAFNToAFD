using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFNToAFD
{
    class Program
    {
        static char Empty = '&';
        static List<string> Start = new List<string>();
        static List<string> Final = new List<string>();
        static HashSet<string> Qs = new HashSet<string>();
        static HashSet<string> Alphabets = new HashSet<string>();
        static List<LineTransition> TableTransition = new List<LineTransition>();

        static void Main(string[] args)
        {
            Convert();
        }

        public static void Convert()
        {

            //Console.WriteLine("Digite: ");
            //Console.ReadKey();

            var afn = new List<string>()
            {
                "q0 -a-> q0",
                "q0 -&-> q1",
                "q1 -b-> q1",
                "q1 -&-> q2",
                "q2 -a-> q2"
            };

            Start.Add("q0");

            Qs = GetQs(afn);
            Alphabets = GetAphabets(afn);
            TableTransition = CreateTableTransition(afn);
            ShowTable();
            AFeToAFD();
        }

        #region AFEToAFD

        public static void AFeToAFD()
        {
            var states = Closure();
        }

        public static List<State> Closure()
        {
            var list = new List<State>();

            foreach (var q in Qs)
            {            
                ConcatState
            }

            return list;
        }

        public static string ConcatState(string start)
        {



            return retorno;
        }

        #endregion

        #region TableTransition

        #region ShowTable

        public static void ShowTable()
        {
            //mostrar tabela
        }
 
        #endregion

        #region CreateTableTransition

        public static List<LineTransition> CreateTableTransition(List<string> afn)
        {
            var list = new List<LineTransition>();

            foreach (var path in afn)
            {
                var sequence = new List<QOrder>();

                foreach (var q in Qs)
                {
                    if (path.Contains(q))
                    {
                        sequence.Add(new QOrder { Q = q, Order = path.IndexOf(q) });
                    }
                }
                sequence = sequence.OrderBy(o => o.Order).ToList();

                var line = list.Find(t => t.Q == sequence[0].Q);

                if (line != null)
                {
                    line.Colunm.Add(new ColumnTransition
                    {
                        To = sequence[1].Q,
                        Alphabet = Alphabets.FirstOrDefault(a => path.Contains(a))
                    });
                }
                else
                {
                    line = new LineTransition
                    {
                        Q = sequence[0].Q,
                        Colunm = new List<ColumnTransition>()
                        {
                            new ColumnTransition
                            {
                                To = (sequence.Count == 1) ? sequence[0].Q : sequence[1].Q,
                                Alphabet = Alphabets.FirstOrDefault(a => path.Contains(a))
                            }
                        }
                    };

                    list.Add(line);
                }
            }

            return list;
        }

        #endregion

        #endregion

        #region Get

        #region GetAphabets

        public static HashSet<string> GetAphabets(List<string> afn)
        {
            try
            {
                var hashset = new HashSet<string>();

                for (int i = 0; i < afn.Count; i++)
                {
                    var indexOf = afn[i].IndexOf('-');
                    hashset.Add(afn[i][indexOf + 1].ToString());
                }

                return hashset;
            }
            catch (Exception)
            {
                Console.Write("Erro");
                throw;
            }
        }

        #endregion

        #region GetQs

        public static HashSet<string> GetQs(List<string> afn)
        {
            try
            {
                var hashset = new HashSet<string>();

                for (int i = 0; i < afn.Count; i++)
                {
                    var variable = afn[i][0];
                    var copy = afn[i];

                    while (afn[i].Contains(variable))
                    {
                        var indexOf = afn[i].IndexOf(variable);
                        var count = 0;
                        var remove = string.Empty;

                        try
                        {
                            while (afn[i][indexOf + count] != ' ')
                            {
                                count++;
                            }
                        }
                        catch (Exception) { }

                        remove = afn[i].Substring(indexOf, count);
                        afn[i] = afn[i].Remove(indexOf, count);

                        hashset.Add(remove);
                    }

                    afn[i] = copy;
                }

                return hashset;
            }
            catch (Exception)
            {
                Console.Write("Erro");
                throw;
            }
        }

        #endregion

        #endregion
    }
}
