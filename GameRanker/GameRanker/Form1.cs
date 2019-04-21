using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Linq;

namespace GameRanker
{
    public partial class Form1 : Form
    {
        private List<Button> btnList = new List<Button>();
        private List<int> scoreList = new List<int>();
        private List<Point> pairList = new List<Point>();
        private List<Button> orderedList = new List<Button>();

        public Form1()
        {
            InitializeComponent();
        }

        private void ResetEverything()
        {
            btnList.Clear();
            scoreList.Clear();
            pairList.Clear();
            orderedList.Clear();

            List<Control> itemsToRemove = new List<Control>();
            foreach (Control ctrl in Controls)
            {
                if (ctrl.Tag != null && ctrl.Tag.ToString() == "dispose")
                {
                    itemsToRemove.Add(ctrl);
                }
            }

            foreach (Control ctrl in itemsToRemove)
            {
                Controls.Remove(ctrl);
                ctrl.Dispose();
            }
        }

        private void btnLoadChoices_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Are you sure you want to start over?", "Confirmation", MessageBoxButtons.YesNo); ;
            if (dialog == DialogResult.Yes)
            {
                ResetEverything();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = "C:\\Users\\Desktop\\";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (String file in openFileDialog.FileNames)
                    {
                        try
                        {
                            Image loadedImage = Image.FromFile(file);
                            Button newButton = new Button();
                            newButton.BackgroundImage = Image.FromFile(file);
                            newButton.BackgroundImageLayout = ImageLayout.Zoom;
                            newButton.Visible = false;
                            btnList.Add(newButton);
                            this.Controls.Add(newButton);
                            scoreList.Add(0);
                        }
                        catch (SecurityException ex)
                        {
                            // The user lacks appropriate permissions to read files, discover paths, etc.
                            MessageBox.Show("Security error. Please contact your administrator for details.\n\n" +
                                "Error message: " + ex.Message + "\n\n" +
                                "Details (send to Support):\n\n" + ex.StackTrace
                            );
                        }
                        catch (Exception ex)
                        {
                            // Could not load the image - probably related to Windows file system permissions.
                            MessageBox.Show("Cannot display the image: " + file.Substring(file.LastIndexOf('\\'))
                                + ". You may not have permission to read the file, or " +
                                "it may be corrupt.\n\nReported error: " + ex.Message);
                        }
                    }

                    btnLoadChoices.Text = "Restart";

                    foreach (Button b in btnList)
                    {
                        b.Width = 460;
                        b.Height = 215;
                        b.Top = 151;
                        b.Left = 12;
                        b.Click += MyClick;
                    }

                    CreatePairList(btnList.Count);
                    pairList.Shuffle();
                    Main();
                }
            }
        }

        private void CreatePairList(int count)
        {
            for (int i = 0; i < count; i++)
            {
                for (int j = count - 1; j > i; j--)
                {
                    Point p = new Point(i, j);
                    pairList.Add(p);
                }
            }
        }

        private void Main()
        {
            if (pairList.Count > 0)
            {
                this.Text = "Game Ranker 0.7a - " + pairList.Count.ToString();
                LoadTwoChoices(pairList[0]);
            }
            else
            {
                this.Text = "Game Ranker 0.7a - Final Results";
                MakeOrderedList();
                ShowFinalScores();
            }
        }

        private void LoadTwoChoices(Point p)
        {
            btnList[p.X].Left = 12;
            btnList[p.X].Visible = true;
            btnList[p.Y].Left = 472;
            btnList[p.Y].Visible = true;
        }

        private void MakeOrderedList()
        {
            foreach (Button b in btnList)
            {
                b.Tag = scoreList[btnList.IndexOf(b)].ToString();

                int score = 0;
                if (Int32.TryParse(b.Tag.ToString(), out score))
                {
                    if (score < 10)
                    {
                        b.Tag = "0" + scoreList[btnList.IndexOf(b)].ToString();
                    }
                }
            }
            btnList = btnList.OrderByDescending(b => b.Tag).ToList();
        }

        private void ShowFinalScores()
        {
            for (int i = 0; i < btnList.Count; i++)
            {
                Label l = new Label();
                l.Text = btnList[i].Tag.ToString();
                //l.Text = (i + 1).ToString();
                l.Font = new Font("Arial", 11, FontStyle.Bold);
                l.TextAlign = ContentAlignment.MiddleLeft;
                l.Height = 43;
                l.Width = 15;
                l.Tag = "dispose";

                btnList[i].Height = 43;
                btnList[i].Width = 92;

                if (i < 9)
                {
                    l.Left = 5;
                    l.Top = 15 + (43 * i) + (10 * i);

                    btnList[i].Left = 20;
                    btnList[i].Top = 15 + (43 * i) + (10 * i);
                }
                else if (i < 18)
                {
                    l.Left = 112 + 5;
                    l.Top = 15 + (43 * (i - 9)) + (10 * (i - 9));

                    btnList[i].Left = 112 + 20;
                    btnList[i].Top = 15 + (43 * (i - 9)) + (10 * (i - 9));
                }
                else if (i < 27)
                {
                    l.Left = 112 + 112 + 5;
                    l.Top = 15 + (43 * (i - 18)) + (10 * (i - 18));

                    btnList[i].Left = 112 + 112 + 20;
                    btnList[i].Top = 15 + (43 * (i - 18)) + (10 * (i - 18));
                }
                else
                {
                    l.Left = 112 + 112 + 112 + 5;
                    l.Top = 15 + (43 * (i - 27)) + (10 * (i - 27));

                    btnList[i].Left = 112 + 112 + 112 + 20;
                    btnList[i].Top = 15 + (43 * (i - 27)) + (10 * (i - 27));
                }

                this.Controls.Add(l);
                btnList[i].Enabled = false;
                btnList[i].Visible = true;
                btnList[i].Tag = "dispose";
            }
        }

        private void MyClick(object sender, EventArgs e)
        {
            Button me = (Button)sender;
            foreach (Button b in btnList)
            {
                if (b == me)
                {
                    int myIndex = btnList.IndexOf(b);
                    scoreList[myIndex]++;
                }
                b.Visible = false;
            }
            pairList.RemoveAt(0);
            Main();
        }
    }

    internal static class MyExtensions
    {
        //Stolen code for shuffling Lists
        public static void Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}