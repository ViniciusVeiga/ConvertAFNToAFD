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
        static HashSet<char> Alphabets = new HashSet<char>();
        static List<LineTransition> TableTransition = new List<LineTransition>();
        static HashSet<string> StateName = new HashSet<string>();
        static List<State> State = new List<State>();

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

        #region AFeToAFD

        public static void AFeToAFD()
        {
            State.AddRange(Closure());

            for (int i = 0; i < State.Count; i++)
            {
                foreach (var a in Alphabets)
                {
                    var state = DFAedge(State[i], a);

                    State[i].Columns.Add(new ColumnTransition
                    {
                        To = state.Name,
                        Alphabet = a
                    });

                    if (StateName.Add(state.Name))
                    {
                        State.Add(state);
                    }
                }
            }
        }

        #region DFAedge

        public static State DFAedge(State start, char alphabet)
        {
            var state = new State();

            foreach (var q in start.Qs)
            {
                var line = TableTransition.Find(t => t.Q == q.Q);

                foreach (var colunm in line.Colunm)
                {
                    
                }
            }

            return state;
        }

        #endregion

        #region Closure

        public static List<State> Closure()
        {
            var list = new List<State>();

            foreach (var q in Qs)
            {
                var state = new State { Qs = ConcatState(q) };
                state.CriarNome();

                if (StateName.Add(state.Name))
                {
                    list.Add(state);
                }
            }

            return list;
        }

        public static List<QOrder> ConcatState(string start)
        {
            var list = new List<QOrder> { new QOrder { Q = start } };
            var lineStart = TableTransition.Find(t => t.Q == start);

            foreach (var colunm in lineStart.Colunm)
            {
                if (colunm.Alphabet == Empty)
                {
                    list.AddRange(ConcatState(colunm.To));
                }
            }

            return list;
        }

        #endregion

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
                        sequence.Add(new QOrder { Q = q, OrderForSequence = path.IndexOf(q) });
                    }
                }
                sequence = sequence.OrderBy(o => o.OrderForSequence).ToList();

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

        public static HashSet<char> GetAphabets(List<string> afn)
        {
            try
            {
                var hashset = new HashSet<char>();

                for (int i = 0; i < afn.Count; i++)
                {
                    var indexOf = afn[i].IndexOf('-');
                    hashset.Add(afn[i][indexOf + 1]);
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
