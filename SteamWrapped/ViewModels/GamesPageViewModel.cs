using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamWrapped.Models;
using SteamWrapped.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace SteamWrapped.ViewModels;

public partial class GamesPageViewModel : ObservableObject
{
    private readonly WrappedService _service = new();

    private List<Game> _allGames = new();

    [ObservableProperty]
    private ObservableCollection<Game> games = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedSort = "По алфавиту";

    [ObservableProperty]
    private string selectedFilter = "Все игры";

    [ObservableProperty]
    private bool isGridView = true;

    public bool IsListView => !IsGridView;

    [RelayCommand]
    private void SetGridView()
    {
        IsGridView = true;
        OnPropertyChanged(nameof(IsListView));
    }

    [RelayCommand]
    private void SetListView()
    {
        IsGridView = false;
        OnPropertyChanged(nameof(IsListView));
    }
    public List<string> SortOptions { get; } =
    [
        "По алфавиту",
        "По времени",
        "По достижениям"
    ];

    public int TotalGamesCount => _allGames.Count;

    public int RecentGamesCount =>
        Math.Min(10, _allGames.Count);

    public int FavoriteGamesCount =>
        _allGames.Count(g => g.HoursPlayed >= 20);

    public int CompletedGamesCount =>
        _allGames.Count(g =>
            g.AchievementsTotal > 0 &&
            g.AchievementsUnlocked >= g.AchievementsTotal);

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnSelectedSortChanged(string value)
    {
        ApplyFilters();
    }
    partial void OnSelectedFilterChanged(string value)
    {
        ApplyFilters();
    }
    public async Task LoadGames(string steamId)
    {
        try
        {
            IsLoading = true;

            var steamGames =
                await _service.GetSteamGames(steamId);

            _allGames = steamGames.ToList();

            OnPropertyChanged(nameof(TotalGamesCount));
            OnPropertyChanged(nameof(RecentGamesCount));
            OnPropertyChanged(nameof(FavoriteGamesCount));
            OnPropertyChanged(nameof(CompletedGamesCount));

            ApplyFilters();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ShowAll()
    {
        SelectedFilter = "Все игры";
        ApplyFilters();
    }

    [RelayCommand]
    private void ShowRecent()
    {
        SelectedFilter = "Недавние";
        ApplyFilters();
    }

    [RelayCommand]
    private void ShowFavorites()
    {
        SelectedFilter = "Избранное";
        ApplyFilters();
    }

    [RelayCommand]
    private void ShowCompleted()
    {
        SelectedFilter = "Пройденные";
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        IEnumerable<Game> query = _allGames;

        switch (SelectedFilter)
        {
            case "Недавние":
                query = query
                    .OrderByDescending(g => g.HoursPlayed)
                    .Take(10);
                break;

            case "Избранное":
                query = query
                    .Where(g => g.HoursPlayed >= 20);
                break;

            case "Пройденные":
                query = query.Where(g =>
                    g.AchievementsTotal > 0 &&
                    g.AchievementsUnlocked >= g.AchievementsTotal);
                break;
        }

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(g =>
                g.Name.Contains(
                    SearchText,
                    StringComparison.OrdinalIgnoreCase));
        }

        query = SelectedSort switch
        {
            "По времени" =>
                query.OrderByDescending(g => g.HoursPlayed),

            "По достижениям" =>
                query.OrderByDescending(g => g.AchievementPercent),

            _ =>
                query.OrderBy(g => g.Name)
        };

        Games.Clear();

        foreach (var game in query)
        {
            Games.Add(game);
        }
    }
    public GamesPageViewModel()
    {
        ThemeService.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(bool _)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            OnPropertyChanged(nameof(SelectedFilter));
            OnPropertyChanged(nameof(IsGridView));
            OnPropertyChanged(nameof(IsListView));
        });
    }
}