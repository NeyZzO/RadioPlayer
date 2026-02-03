using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RadioPlayer.Controls;

public partial class RadioStationSlider : UserControl {
    private const double ScrollAmount = 400;
    private ScrollViewer? _scrollViewer;

    public RadioStationSlider() {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        _scrollViewer = this.FindControl<ScrollViewer>("StationsScrollViewer");
    }

    private void ScrollLeft_Click(object? sender, RoutedEventArgs e) {
        if (_scrollViewer == null) return;

        var currentOffset = _scrollViewer.Offset;
        var newOffset = currentOffset.X - ScrollAmount;
        if (newOffset < 0) newOffset = 0;
        _scrollViewer.Offset = new Avalonia.Vector(newOffset, currentOffset.Y);
    }

    private void ScrollRight_Click(object? sender, RoutedEventArgs e) {
        if (_scrollViewer == null) return;

        var currentOffset = _scrollViewer.Offset;
        var maxOffset = _scrollViewer.Extent.Width - _scrollViewer.Viewport.Width;
        var newOffset = currentOffset.X + ScrollAmount;
        if (newOffset > maxOffset) newOffset = maxOffset;
        _scrollViewer.Offset = new Avalonia.Vector(newOffset, currentOffset.Y);
    }
}
