using HomeInventory.Client.Models;
using HomeInventory.Desktop.Wpf.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace HomeInventory.Desktop.Wpf.Views
{
    public partial class TagPickerWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<TagChoiceVm> TagChoices { get; } = [];
        public ObservableCollection<ColorChoiceVm> AvailableColors { get; }
        public bool HasNoTags => TagChoices.Count == 0;
        public TagPickerResult? Result { get; private set; }

        private readonly bool _itemMode;
        private readonly List<Guid> _tagIdsToDelete = [];
        private ColorChoiceVm? _selectedColor;

        private static readonly (string Hex, string Name)[] _colors =
        [
            ("#EF4444", "Červená"), ("#F97316", "Oranžová"), ("#EAB308", "Žlutá"), ("#22C55E", "Zelená"),
            ("#3B82F6", "Modrá"), ("#8B5CF6", "Fialová"), ("#EC4899", "Růžová"), ("#6B7280", "Šedá")
        ];

        public TagPickerWindow(string header, IReadOnlyList<Tag> allTags, IReadOnlyList<Guid> currentTagIds, bool itemMode)
        {
            InitializeComponent();
            DataContext = this;
            HeaderText.Text = header;
            _itemMode = itemMode;

            foreach (var tag in allTags)
                TagChoices.Add(new TagChoiceVm(tag.Id, tag.Name, tag.Color, currentTagIds.Contains(tag.Id), canDelete: true));

            AvailableColors = new ObservableCollection<ColorChoiceVm>(
                _colors.Select(c => new ColorChoiceVm(c.Hex, c.Name)));
            _selectedColor = AvailableColors[0];
            _selectedColor.IsSelected = true;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Result = new TagPickerResult
            {
                FinalSelectedTagIds = TagChoices
                    .Where(t => t.IsSelected && !t.IsNew)
                    .Select(t => t.Id).ToList(),
                NewTagsToCreate = TagChoices
                    .Where(t => t.IsNew)
                    .Select(t => new TagPickerResult.NewTagSpec(t.Name, t.Color)).ToList(),
                TagIdsToDelete = [.. _tagIdsToDelete]
            };
            DialogResult = true;
        }

        private void DeleteTag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is TagChoiceVm vm)
            {
                if (MessageBox.Show($"Smazat štítek '{vm.Name}'?\nBude odebrán ze všech položek.",
                        "Smazat štítek", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _tagIdsToDelete.Add(vm.Id);
                    TagChoices.Remove(vm);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasNoTags)));
                }
            }
        }

        private void AddTag_Click(object sender, RoutedEventArgs e)
        {
            var name = NewTagName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Zadejte název štítku.", "Chyba");
                return;
            }
            if (name.Length > 30)
            {
                MessageBox.Show("Název štítku může mít nejvýše 30 znaků.", "Chyba");
                return;
            }

            var color = _selectedColor?.Hex ?? "#6B7280";
            var newTag = new TagChoiceVm(Guid.NewGuid(), name, color, isSelected: true, canDelete: false) { IsNew = true };
            TagChoices.Add(newTag);
            NewTagName.Clear();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasNoTags)));
        }

        private void ColorChoice_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string hex)
            {
                if (_selectedColor is not null) _selectedColor.IsSelected = false;
                _selectedColor = AvailableColors.FirstOrDefault(c => c.Hex == hex);
                if (_selectedColor is not null) _selectedColor.IsSelected = true;
            }
        }
    }

    public class TagChoiceVm : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Guid Id { get; }
        public string Name { get; }
        public string Color { get; }
        public bool CanDelete { get; }
        public bool IsNew { get; init; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; PropertyChanged?.Invoke(this, new(nameof(IsSelected))); }
        }

        public TagChoiceVm(Guid id, string name, string color, bool isSelected, bool canDelete)
        {
            Id = id; Name = name; Color = color; _isSelected = isSelected; CanDelete = canDelete;
        }
    }

    public class ColorChoiceVm : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string Hex { get; }
        public string Name { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; PropertyChanged?.Invoke(this, new(nameof(IsSelected))); }
        }

        public ColorChoiceVm(string hex, string name) { Hex = hex; Name = name; }
    }
}
