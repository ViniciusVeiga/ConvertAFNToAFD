using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFNToAFD
{
    class Program
    {
        #region Variables

        static char Empty = '&';
        static List<string> Start = new List<string>();
        static List<string> Final = new List<string>();
        static HashSet<string> Qs = new HashSet<string>();
        static HashSet<char> Alphabets = new HashSet<char>();
        static List<LineTransition> TableTransition = new List<LineTransition>();
        static HashSet<string> StateName = new HashSet<string>();
        static List<State> State = new List<State>();

        #endregion

        static void Main(string[] args)
        {
            Convert();

            Console.ReadKey();
        }

        public static void Convert()
        {
            var afn = Menu();
            Qs = GetQs(afn);
            Alphabets = GetAphabets(afn);
            TableTransition = CreateTableTransition(afn);
            AFeToAFD();
            Show();
        }

        #region Menu

        public static List<string> Menu()
        {
            var afn = new List<string>();
            var value = string.Empty;

            Console.WriteLine("Exemplo AFe digitado (S: Start, F: Final): ");
            Console.WriteLine("q0 -a-> q0");
            Console.WriteLine("q0 -&-> q1");
            Console.WriteLine("q1 -b-> q1");
            Console.WriteLine("q1 -&-> q2");
            Console.WriteLine("q2 -a-> q2");

            Console.WriteLine("\nIniciais: ");
            Console.WriteLine("q0");

            Console.WriteLine("\nFinais: ");
            Console.WriteLine("q2");

            Console.WriteLine("\nDigite 'Finalizado' para sair.");
            Console.WriteLine("Digite 'Exemplo' para ver o resultado do exemplo.");
            Console.WriteLine("Digite o AFe: ");
            var sair = false;

            while (sair == false)
            {
                value = Console.ReadLine();

                switch (value)
                {
                    case "Finalizado":
                        sair = true;
                        break;

                    case "Exemplo":
                        afn = new List<string>()
                        {
                            "q0 -a-> q0",
                            "q0 -&-> q1",
                            "q1 -b-> q1",
                            "q1 -&-> q2",
                            "q2 -a-> q2"
                        };

                        Start.Add("q0");
                        Final.Add("q2");

                        sair = true;
                        break;
                    default:
                        afn.Add(value);
                        break;
                }

            }

            if (value != "Exemplo")
            {
                Console.WriteLine("\nDigite os iniciais, separados por ,: ");
                Start = Console.ReadLine().Split(',').ToList();

                Console.WriteLine("\nDigite os iniciais, separados por ,: ");
                Final = Console.ReadLine().Split(',').ToList();
            }

            return afn;
        }

        #endregion

        #region Show

        public static void Show()
        {
            Console.WriteLine("\nAFD:");
            foreach (var status in State)
            {
                if (Start.Any(n => status.Name.Contains(n)))
                {
                    status.Name = $"{status.Name} I";
                }

                if (Final.Any(n => status.Name.Contains(n)))
                {
                    status.Name = $"{status.Name} F";
                }

                foreach (var column in status.Columns)
                {
                    if (Start.Any(n => column.To.Contains(n)))
                    {
                        column.To = $"{column.To} I";
                    }

                    if (Final.Any(n => column.To.Contains(n)))
                    {
                        column.To = $"{column.To} F";
                    }

                    Console.WriteLine($"{status.Name} -{column.Alphabet}-> {column.To}");
                }
            }
        }

        #endregion

        #region AFeToAFD

        public static void AFeToAFD()
        {
            State.AddRange(Closure());

            for (int i = 0; i < State.Count; i++)
            {
                foreach (var a in Alphabets.Where(e => e != Empty))
                {
                    var state = DFAedge(State[i], a);

                    state.CreateName();

                    if (state.Name != null)
                    {
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
        }

        #region DFAedge

        public static State DFAedge(State start, char alphabet)
        {
            var state = new State();
            var names = new HashSet<string>();

            state.Qs = new List<QOrder>();

            foreach (var q in start.Qs)
            {
                var line = TableTransition.Find(t => t.Q == q.Q);

                if (line != null)
                {
                    foreach (var colunm in line.Colunm)
                    {
                        if ($"{alphabet}{Empty}".Contains(colunm.Alphabet))
                        {
                            if (names.Add(colunm.To))
                            {
                                state.Qs.Add(new QOrder { Q = colunm.To });
                            }
                        }
                    }
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
                state.CreateName();

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

            if (lineStart != null)
            {
                foreach (var colunm in lineStart.Colunm)
                {
                    if (colunm.Alphabet == Empty)
                    {
                        list.AddRange(ConcatState(colunm.To));
                    }
                }
            }

            return list;
        }

        #endregion

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
