using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFNToAFD
{
    class Program
    {
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
                "q0 -a-> q1",
                "q0 -a-> q2",
                "q1 -b-> q0",
                "q1 -a-> q2",
                "q1 -b-> q2",
                "q2 -a-> q2",
                "q2 -a-> q0"
            };

            Qs = GetQs(afn);
            Alphabets = GetAphabets(afn);
            TableTransition = CreateTableTransition(afn);

        }

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

                foreach (var o in sequence.OrderBy(o => o.Order))
                {
                    list.Add(new LineTransition { })
                }
            }

            return list;
        }

        #endregion

        #region Gets

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
