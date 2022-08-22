using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace WaveFunctionCollapseLib
{
    /// <summary>
    /// Wave Function Collapse game based on some item type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WaveFunctionCollapse<T> where T : class
    {
        public long X { get; }
        public long Y { get; }
        private readonly Random rnd;
        private readonly bool fastWay;
        private readonly bool[][] waveVisits;
        private readonly IVisitor<T> visitor;
        private List<WaveFunctionCollapseItem<T>>[][] boardOutcomes;
        public List<WaveFunctionCollapseItem<T>> Possiblities { get; }
        public WaveFunctionCollapseItem<T>?[][] Board { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="possiblities"></param>
        /// <param name="visitor"></param>
        /// <param name="fastWay"></param>
        public WaveFunctionCollapse(long x, long y, List<WaveFunctionCollapseItem<T>> possiblities, IVisitor<T> visitor, bool fastWay = true)
        {
            //Get Configurations From appsettings.json
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

            //Initialize Logs
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            //Initialize Parameters
            this.X = x;
            this.Y = y;
            this.visitor = visitor;
            this.fastWay = fastWay;
            this.Possiblities = possiblities;

            //Initialize Defaults
            this.rnd = new Random();
            waveVisits = new bool[X][];
            Board = new WaveFunctionCollapseItem<T>[X][];
            boardOutcomes = new List<WaveFunctionCollapseItem<T>>[X][];

            //Fill Arrays
            InitializeBoardAndOutcomes();

            Log.Information("Create Wave Function with info: grid({X},{Y}) {fastWay} Nº possiblities->{possiblities}}", X, Y, fastWay, possiblities);
        }

        /// <summary>
        /// Initialize all positions in the matrix
        /// </summary>
        private void InitializeBoardAndOutcomes()
        {
            for (long xAxis = 0; xAxis < X; xAxis++)
            {
                Board[xAxis] = new WaveFunctionCollapseItem<T>[Y];
                boardOutcomes[xAxis] = new List<WaveFunctionCollapseItem<T>>[Y];
                waveVisits[xAxis] = new bool[Y];
                for (long yAxis = 0; yAxis < Y; yAxis++)
                {
                    Board[xAxis][yAxis] = null;
                    boardOutcomes[xAxis][yAxis] = Possiblities.ToList();
                    waveVisits[xAxis][yAxis] = false;
                }
            }
        }

        /// <summary>
        /// Visit all board
        /// </summary>
        public void DrawBoard()
        {
            visitor.BeforeVisit();

            for (long xAxis = 0; xAxis < X; xAxis++)
                for (long yAxis = 0; yAxis < Y; yAxis++)
                    Visit(xAxis, yAxis, false);
        }

        /// <summary>
        /// Start wave at x/y position (recursive)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void StartWave(long x, long y)
        {
            //Validate coordinate
            if (CoordinateFilled(x, y))
                return;

            //Log
            Log.Information("Start waving at ({x},{y}): {TOP} {LEFT} {BOTTOM} {RIGTH}", x, y,
                InvalidCoordinate(x - 1, y) ? null : Board[x - 1][y],
                InvalidCoordinate(x, y - 1) ? null : Board[x][y - 1],
                InvalidCoordinate(x + 1, y) ? null : Board[x + 1][y],
                InvalidCoordinate(x, y + 1) ? null : Board[x][y + 1]);

            //If it's fast way
            if (fastWay)
            {
                //calculate possibilites from all possibilites available
                var temp = new List<WaveFunctionCollapseItem<T>>();
                foreach (var possible in Possiblities)
                {
                    if (VerifySides(possible, x, y))
                        temp.Add(possible);
                }

                //set outcome
                boardOutcomes[x][y] = temp;

                //Log
                Log.Information("Available possibilities for ({x},{y}): {possibilities}", x, y, temp);
            }

            //Couldn't end
            if (boardOutcomes[x][y].Count <= 0)
                throw new ArgumentOutOfRangeException("There is no way to complete this generation!", new ArgumentOutOfRangeException());

            //Get random outcome
            var outcome = boardOutcomes[x][y]?[rnd.Next(boardOutcomes[x][y].Count)];

            if (outcome == null)
                return;

            Board[x][y] = outcome;
            Log.Information("Random choosed for ({x},{y}): {item}", x, y, outcome);

            //If not fast way Generate all possible outcomes from now
            if (!fastWay)
            {
                ClearWaveVisits();
                RecalculateOutcomes(x - 1, y);
                RecalculateOutcomes(x, y - 1);
                RecalculateOutcomes(x + 1, y);
                RecalculateOutcomes(x, y + 1);
            }
            else // if its fast way just visit
                Visit(x, y, true);

            //Generate Random direction pattern
            switch (rnd.Next(3))
            {
                case 0:
                    StartWave(x - 1, y);
                    StartWave(x, y - 1);
                    StartWave(x + 1, y);
                    StartWave(x, y + 1);
                    break;
                case 1:
                    StartWave(x, y - 1);
                    StartWave(x + 1, y);
                    StartWave(x, y + 1);
                    StartWave(x - 1, y);
                    break;
                case 2:
                    StartWave(x + 1, y);
                    StartWave(x, y + 1);
                    StartWave(x, y - 1);
                    StartWave(x - 1, y);
                    break;
                case 3:
                    StartWave(x, y + 1);
                    StartWave(x, y - 1);
                    StartWave(x + 1, y);
                    StartWave(x - 1, y);
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Calculate possible outcomes (recursive)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void RecalculateOutcomes(long x, long y)
        {
            //check if coordinate is already filled
            if (CoordinateFilled(x, y)) return;

            //check if already recalculated
            if (IsVisited(x, y)) return;

            //Verify items that are a possibility
            var temp = new List<WaveFunctionCollapseItem<T>>();
            foreach (var outcome in Possiblities)
            {
                if (VerifySides(outcome, x, y))
                    temp.Add(outcome);
            }

            boardOutcomes[x][y] = temp;

            visiting.Add(new KeyValuePair<long, long>(x, y));
            Visit(x, y, true);

            //if its all possibilites don't need to propagate
            if (boardOutcomes[x][y].Count == Possiblities.Count)
                return;

            //propagate
            RecalculateOutcomes(x - 1, y);
            RecalculateOutcomes(x, y - 1);
            RecalculateOutcomes(x + 1, y);
            RecalculateOutcomes(x, y + 1);
        }

        #region Validations
        /// <summary>
        /// Validate if item is valid in x/y position
        /// </summary>
        /// <param name="outcome"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool VerifySides(WaveFunctionCollapseItem<T> outcome, long x, long y)
        {
            bool valid = true;
            //Verify Left
            if (!InvalidCoordinate(x, y + 1) && Board[x][y + 1] is not null)
                valid = (Board[x][y + 1]?.ValidSide(Orientations.LEFT, outcome.MyTypes[Orientations.RIGHT]) ?? true) && valid;

            //Verify Right
            if (!InvalidCoordinate(x, y - 1) && Board[x][y - 1] is not null)
                valid = (Board[x][y - 1]?.ValidSide(Orientations.RIGHT, outcome.MyTypes[Orientations.LEFT]) ?? true) && valid;

            //Verify Top
            if (!InvalidCoordinate(x + 1, y) && Board[x + 1][y] is not null)
                valid = (Board[x + 1][y]?.ValidSide(Orientations.TOP, outcome.MyTypes[Orientations.BOTTOM]) ?? true) && valid;

            //Verify Bottom
            if (!InvalidCoordinate(x - 1, y) && Board[x - 1][y] is not null)
                valid = (Board[x - 1][y]?.ValidSide(Orientations.BOTTOM, outcome.MyTypes[Orientations.TOP]) ?? true) && valid;

            return valid;
        }
        /// <summary>
        /// Validate x/y position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool InvalidCoordinate(long x, long y)
        {
            return x < 0 || y < 0 || x >= X || y >= Y;
        }

        /// <summary>
        ///  Verify if x/y position is filled
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool CoordinateFilled(long x, long y)
        {
            if (InvalidCoordinate(x, y))
                return true;

            return Board[x][y] is not null;
        }
        #endregion

        #region Visiting Logic
        private void ClearWaveVisits()
        {
            foreach (var item in visiting)
                Visit(item.Key, item.Value, Board[item.Key][item.Value] is not null);

            visiting.Clear();
        }
        private bool IsVisited(long x, long y)
        {
            if (CoordinateFilled(x, y))
                return false;

            return waveVisits[x][y];
        }

        private List<KeyValuePair<long, long>> visiting = new List<KeyValuePair<long, long>>();
        private void Visit(long x, long y, bool value)
        {
            if (x < 0 || y < 0 || x >= X || y >= Y)
                return;

            waveVisits[x][y] = value;
            visitor.Visit(Board[x][y]?.Value, boardOutcomes[x][y]?.Count ?? 0, x, y, fastWay);
        }
        #endregion
    }
}
