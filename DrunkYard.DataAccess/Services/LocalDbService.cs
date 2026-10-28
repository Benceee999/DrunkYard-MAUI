using DrunkYard.DataAccess.Models;
using SQLite;

namespace DrunkYard.DataAccess.Services
{
    public class LocalDbService
    {
        private const string DbName = "DrunkYard.db3";
        private readonly SQLiteAsyncConnection _connection;
        private bool _isInitialized = false;

        public LocalDbService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, DbName);
            var flags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;
            _connection = new SQLiteAsyncConnection(dbPath, flags);
        }

        private async Task Init()
        {
            if (_isInitialized)
                return;

            await _connection.CreateTableAsync<User>();
            await _connection.CreateTableAsync<DrinkGame>();
            await _connection.CreateTableAsync<MiniGameItem>();
            await _connection.CreateTableAsync<CandidateGame>();

            var gamesCount = await _connection.Table<DrinkGame>().CountAsync();
            if (gamesCount == 0)
            {
                await SeedGamesAsync();
            }

            var questionsCount = await _connection.Table<MiniGameItem>().CountAsync();
            if (questionsCount == 0)
            {
                await SeedQuestionsAsync();
            }

            var candidatesCount = await _connection.Table<CandidateGame>().CountAsync();
            if (candidatesCount == 0)
            {
                await SeedCandidatesAsync();
            }

            _isInitialized = true;
        }

        public async Task<List<DrinkGame>> GetGamesAsync()
        {
            await Init();
            return await _connection.Table<DrinkGame>().ToListAsync();
        }

        public async Task<DrinkGame> GetGameByIdAsync(int id)
        {
            await Init();
            return await _connection.Table<DrinkGame>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateGameAsync(DrinkGame game)
        {
            await Init();
            await _connection.InsertAsync(game);
        }

        public async Task UpdateGameAsync(DrinkGame game)
        {
            await Init();
            await _connection.UpdateAsync(game);
        }

        public async Task DeleteGameAsync(DrinkGame game)
        {
            await Init();
            await _connection.DeleteAsync(game);
        }

        public async Task<List<MiniGameItem>> GetMiniGameItemsAsync(MiniGameType type)
        {
            await Init();
            return await _connection.Table<MiniGameItem>().Where(x => x.Type == type).ToListAsync();
        }

        public async Task<List<CandidateGame>> GetCandidatesAsync()
        {
            await Init();
            return await _connection.Table<CandidateGame>().ToListAsync();
        }

        public async Task SaveCandidateAsync(CandidateGame candidate)
        {
            await Init();
            if (string.IsNullOrEmpty(candidate.Id))
                candidate.Id = Guid.NewGuid().ToString();

            await _connection.InsertOrReplaceAsync(candidate);
        }

        public async Task UpdateCandidateAsync(CandidateGame candidate)
        {
            await Init();
            await _connection.UpdateAsync(candidate);
        }

        public async Task SaveCandidatesFromApiAsync(List<CandidateGame> remoteCandidates)
        {
            await Init();
            foreach (var remote in remoteCandidates)
            {
                var local = await _connection.Table<CandidateGame>()
                                .Where(x => x.Id == remote.Id || x.Title == remote.Title)
                                .FirstOrDefaultAsync();

                if (local == null)
                {
                    await _connection.InsertAsync(remote);
                }
                else
                {
                    if (remote.Votes > local.Votes)
                    {
                        local.Votes = remote.Votes;
                    }
                    local.Title = remote.Title;
                    local.Description = remote.Description;
                    local.Rules = remote.Rules;
                    local.Category = remote.Category;
                    local.MinPlayers = remote.MinPlayers;
                    local.IsEquipmentNeeded = remote.IsEquipmentNeeded;
                    await _connection.UpdateAsync(local);
                }
            }
        }

        private async Task SeedGamesAsync()
        {
            var games = new List<DrinkGame>
            {
                new DrinkGame
                {
                    Title = "Én még sosem...",
                    Description = "A klasszikus vallomástevő játék. Ha csináltad már, iszol!",
                    Rules = "Valaki felolvas egy állítást. Aki már tette azt a dolgot, iszik egy kortyot.",
                    Category = GameCategory.Easy,
                    MinPlayers = 2,
                    IsEquipmentNeeded = false,
                    IsFavorite = false,
                    PlayableLogicId = "swipe"
                },
                new DrinkGame
                {
                    Title = "King's Cup",
                    Description = "Húzz egy kártyát, és kövesd a szabályt!",
                    Rules = "Minden kártyaszámhoz tartozik egy szabály (pl. 7-es: Ég a keze).",
                    Category = GameCategory.Medium,
                    MinPlayers = 3,
                    IsEquipmentNeeded = true,
                    IsFavorite = true,
                    PlayableLogicId = "kingscup"
                },
                new DrinkGame
                {
                    Title = "Sörpong",
                    Description = "Ügyességi játék poharakkal és labdával.",
                    Rules = "Dobjátok a labdát az ellenfél poharába...",
                    Category = GameCategory.Medium,
                    MinPlayers = 2,
                    IsEquipmentNeeded = true,
                    PlayableLogicId = null
                },
                new DrinkGame
                {
                    Title = "Felelsz vagy Iszol",
                    Description = "Kínos kérdések vagy ivás.",
                    Rules = "Tegyél fel egy kérdést a tőled balra ülőnek...",
                    Category = GameCategory.Easy,
                    MinPlayers = 2,
                    IsEquipmentNeeded = false,
                    PlayableLogicId = null
                },
                new DrinkGame
                {
                    Title = "Bivaly",
                    Description = "Egész estés játék, figyelem kell hozzá!",
                    Rules = "Senki nem ihat jobb kézzel (vagy a domináns kezével)...",
                    Category = GameCategory.Hard,
                    MinPlayers = 2,
                    IsEquipmentNeeded = false,
                    PlayableLogicId = null
                },
                new DrinkGame
                {
                    Title = "Buszsofőr",
                    Description = "Gyors kártyajáték, ahol a logika dönt.",
                    Rules = "Tippeld meg, hogy a következő lap piros vagy fekete...",
                    Category = GameCategory.Extreme,
                    MinPlayers = 2,
                    IsEquipmentNeeded = true,
                    PlayableLogicId = null
                }
            };

            await _connection.InsertAllAsync(games);
        }

        private async Task SeedCandidatesAsync()
        {
            var candidates = new List<CandidateGame>
            {
                new CandidateGame
                {
                    Id = "local_candidate_1",
                    Title = "Flunkyball",
                    Description = "Kültéri dobójáték sörösüveggel.",
                    Rules = "Két csapat feláll egymással szemben, középen egy flakonnal. Aki eltalálja a labdával, az ihat, amíg a másik csapat vissza nem állítja a flakont.",
                    Category = GameCategory.Medium,
                    MinPlayers = 4,
                    IsEquipmentNeeded = true,
                    Votes = 5
                },
                new CandidateGame
                {
                    Id = "local_candidate_2",
                    Title = "Királyválasztó",
                    Description = "Gyors reakciójáték az asztal körül.",
                    Rules = "Valaki mond egy számot 1 és 10 között. Aki a legközelebb tippel, hozhat egy szabályt a körre.",
                    Category = GameCategory.Easy,
                    MinPlayers = 3,
                    IsEquipmentNeeded = false,
                    Votes = 3
                }
            };

            await _connection.InsertAllAsync(candidates);
        }

        private async Task SeedQuestionsAsync()
        {
            var questions = new List<MiniGameItem>
            {
                new MiniGameItem { Text = "Én még sosem rúgtam be a szüleim előtt.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem csókolóztam idegennel.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem felejtettem el egy egész estét.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem ébredtem úgy, hogy nem tudtam, hol vagyok.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem küldtem részeg üzenetet az exemnek.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem hánytam le valakit véletlenül.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem aludtam el tömegközlekedésen.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem táncoltam asztalon.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem dobtak ki szórakozóhelyről.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem ittam alkoholt reggel 10 előtt.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem néztem bele titokban a párom telefonjába.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem hazudtam a koromról.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem stalkoltam a barátom/barátnőm volt párját Facebookon/Instán.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem használtam Tindert.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem csináltam olyat az ágyban, amit később megbántam.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem voltam szerelmes tanáromba.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem kaptam kosarat nyilvánosan.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem pisiltem bele a medencébe.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem lógtam el a munkából/iskolából kamu betegség miatt.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem loptam semmit (akár egy bolti cukorkát sem).", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem mentem neki üvegajtónak.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem integettem vissza valakinek, aki nem nekem köszönt.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem küldtem pikáns képet rossz embernek.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem mentem fizetés nélkül étteremből/kocsmából.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem fürödtem meztelenül a szabadban.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem törtem össze más telefonját.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem hazudtam ebben a játékban, csak hogy ihassak.", Type = MiniGameType.NeverHaveIEver },
                new MiniGameItem { Text = "Én még sosem írtam azt, hogy „mindjárt ott vagyok”, miközben még el sem indultam.", Type = MiniGameType.NeverHaveIEver }
            };

            await _connection.InsertAllAsync(questions);
        }

        public async Task SaveGamesFromApiAsync(List<DrinkGame> remoteGames)
        {
            await Init();

            foreach (var remoteGame in remoteGames)
            {
                var localGame = await _connection.Table<DrinkGame>()
                                .Where(x => x.Title == remoteGame.Title)
                                .FirstOrDefaultAsync();

                if (localGame == null)
                {
                    remoteGame.RemoteId = remoteGame.Id.ToString();
                    remoteGame.Id = 0;
                    await _connection.InsertAsync(remoteGame);
                }
                else
                {
                    localGame.Description = remoteGame.Description;
                    localGame.Rules = remoteGame.Rules;
                    localGame.Category = remoteGame.Category;
                    localGame.MinPlayers = remoteGame.MinPlayers;
                    localGame.RemoteId = remoteGame.Id.ToString();
                    await _connection.UpdateAsync(localGame);
                }
            }
        }
    }
}