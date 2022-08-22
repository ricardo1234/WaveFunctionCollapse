using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WaveFunctionCollapse.imageItem;
using WaveFunctionCollapseLib;

namespace WaveFunctionCollapse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WaveFunctionCollapse<ImageItem>? Game { get; set; }

        public MainWindow()
        {
            InitializeComponent();            
        }

        /// <summary>
        /// Load settings json with rules
        /// </summary>
        /// <returns></returns>
        private List<WaveFunctionCollapseItem<ImageItem>> LoadJsonRules()
        {
            var result = new List<WaveFunctionCollapseItem<ImageItem>>();

            using (StreamReader r = new StreamReader("Rules.json"))
            {
                string json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<List<ImageItemImportRule>>(json);
                if (items is null) return result;

                var builder = new WaveFunctionCollapseItemBuilder<ImageItem>();
                
                foreach (var item in items)
                {
                    ImageSource source = (ImageSource)new ImageSourceConverter().ConvertFromString(item.Img);
                    if(source is null) continue;

                    foreach (var rotation in item.Rotations)
                    {
                        var image = new ImageItem() { Image = source, Rotation = rotation.RotationDegree, Width = item.Width, Height = item.Height };

                        result.Add(builder.WithItem(image)
                               .WithTopNeighbourType(rotation.SideRules[(int)Orientations.TOP].ToArray())
                               .WithRightNeighbourType(rotation.SideRules[(int)Orientations.RIGHT].ToArray())
                               .WithBottomNeighbourType(rotation.SideRules[(int)Orientations.BOTTOM].ToArray())
                               .WithLeftNeighbourType(rotation.SideRules[(int)Orientations.LEFT].ToArray())
                               .Build());
                    }
                }
            }
            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Convert.ToInt32(X.Text); i++)
                field.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            for (int i = 0; i < Convert.ToInt32(Y.Text); i++)
                field.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                    
            //create the game
            Game = new WaveFunctionCollapse<ImageItem>(long.Parse(X.Text), long.Parse(Y.Text), LoadJsonRules(), new ItemVisitor(field), FastWay.IsChecked ?? true);

            Game.DrawBoard();

            try
            {
                //start game at the middle field
                Game.StartWave(Convert.ToInt64(long.Parse(X.Text) / 2), Convert.ToInt64(long.Parse(Y.Text) / 2));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
