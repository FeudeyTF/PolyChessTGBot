using Newtonsoft.Json;

namespace PolyChessTGBot
{
    public class ConfigFile
    {
        public string BotToken = string.Empty;

        public long QuestionChannel;

        public long CreativeTaskChannel;

        public string LogsFolder = "Logs";

        public string DatabasePath = "data.sqlite";

        public string BotAuthor = string.Empty;

        public List<int> DebugChats = [];

        public List<string> TopPlayers = [];

        public List<string> TeamsWithTournaments = [];

        public List<string> InstitutesTeamsIDs = [];

        public string SemesterStartDate = string.Empty;

        public string SemesterEndDate = string.Empty;

        public List<string> UnnecessaryTournaments = [];

        public bool ShowApiResponseLogs = false;

        public bool ShowLichessApiSending = false;

        public bool ShowButtonInteractLogs = false;

        public SocketSettings Socket = new();

        public int[] SkippingApiRequestErrors = [];

        public long[] Admins = [];

        public List<string> Flairs = [];

        public TestSettings Test = new();

        public Dictionary<string, TournamentScoreRule> TournamentRules = [];

        public static ConfigFile Load(string name)
        {
            ConfigFile emptyConfig = new();
            var path = emptyConfig.Save(name);
            var parsedConfig = JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(path));
            if (parsedConfig != null)
                return parsedConfig;
            return emptyConfig;
        }

        public string Save(string name, bool rewrite = false)
        {
            string configFolder = Path.Combine(Environment.CurrentDirectory, "Configs");
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);
            string path = Path.Combine(configFolder, name + ".json");
            if (!File.Exists(path) || rewrite)
                File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
            return path;
        }
    }

    public class TestSettings
    {
        public int RequiredTournamentsCount;

        public int RequiredVisitedLessonsPercent;

        public int RequiredPuzzlesSolved;
    }

    public class SocketSettings
    {
        public bool StartSocketServer;

        public int Port = 8081;
    }

    public class TournamentScoreRule
    {
        public const int DEFAULT_POINTS_FOR_WINNING = 1;

        public const int DEFAULT_POINTS_FOR_BEING = 1;

        public int PointsForWinning = DEFAULT_POINTS_FOR_WINNING;

        public int PointsForBeing = DEFAULT_POINTS_FOR_BEING;
    }
}