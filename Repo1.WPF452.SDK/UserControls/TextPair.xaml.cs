using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using AutoDependencyPropertyMarker;

namespace Repo1.WPF452.SDK.UserControls
{
    [AutoDependencyProperty]
    public partial class TextPair : UserControl
    {
        public TextPair()
        {
            InitializeComponent();

            Text1Wrapping = TextWrapping.Wrap;
            Text2Wrapping = TextWrapping.Wrap;

            Text1Alignment = TextAlignment.Right;
            Text2Alignment = TextAlignment.Left;

            Text1Weight = FontWeights.Medium;
            Text2Weight = FontWeights.Medium;

            txt1.Bind(nameof(Text1), TextBlock.TextProperty);
            txt2.Bind(nameof(Text2), TextBlock.TextProperty);

            txt1.Bind(nameof(Text1Alignment), TextBlock.TextAlignmentProperty);
            txt2.Bind(nameof(Text2Alignment), TextBlock.TextAlignmentProperty);

            txt1.Bind(nameof(Text1Wrapping), TextBlock.TextWrappingProperty);
            txt2.Bind(nameof(Text2Wrapping), TextBlock.TextWrappingProperty);

            txt1.Bind(nameof(Text1Brush), TextBlock.ForegroundProperty);
            txt2.Bind(nameof(Text2Brush), TextBlock.ForegroundProperty);

            txt1.Bind(nameof(Text1Weight), TextBlock.FontWeightProperty);
            txt2.Bind(nameof(Text2Weight), TextBlock.FontWeightProperty);

            txt1.Bind(nameof(Text1FontStyle), TextBlock.FontStyleProperty);
            txt2.Bind(nameof(Text2FontStyle), TextBlock.FontStyleProperty);

            Loaded += (s, e) =>
            {
                if (GapWidth.GridUnitType == GridUnitType.Auto)
                    GapWidth = new GridLength(7);

                if (Text1Brush == null) Text1Brush = Brushes.Gray;
                if (Text2Brush == null) Text2Brush = Brushes.Black;
            };
        }

        public string         Text1           { get; set; }
        public string         Text2           { get; set; }
                                              
        public GridLength     Text1Width      { get; set; }
        public GridLength     GapWidth        { get; set; }
        public GridLength     Text2Width      { get; set; }

        public TextAlignment  Text1Alignment  { get; set; }
        public TextAlignment  Text2Alignment  { get; set; }

        public TextWrapping   Text1Wrapping   { get; set; }
        public TextWrapping   Text2Wrapping   { get; set; }

        public Brush          Text1Brush      { get; set; }
        public Brush          Text2Brush      { get; set; }

        public FontWeight     Text1Weight     { get; set; }
        public FontWeight     Text2Weight     { get; set; }

        public FontStyle      Text1FontStyle  { get; set; }
        public FontStyle      Text2FontStyle  { get; set; }
    }


    internal static class TextBlockExtensions
    {
        public static void Bind(this FrameworkElement elm, string path, DependencyProperty dependencyProp)
        {
            var binding = new Binding(path);
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(TextPair), 1);
            elm.SetBinding(dependencyProp, binding);
        }
    }
}
