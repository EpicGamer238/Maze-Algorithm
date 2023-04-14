using System.Runtime.InteropServices;

namespace TEST
{
    public partial class Form1 : Form
    {
        List<Control> activeTiles = new List<Control>();
        List<Control> visitedTiles = new List<Control>();
        List<String> OpenEndPoints = new List<string>();
        Random rng = new Random();
        Control start;
        Control end;
        bool isEnd = false;
        int totalcost = 1;
        public Form1()
        {
            InitializeComponent();
            MouseDownFilter mouseFilter = new MouseDownFilter(this);
            mouseFilter.FormClicked += mouseFilter_FormClicked;
            mouseFilter.RFormClicked += mouseFilter_RFormClicked;
            Application.AddMessageFilter(mouseFilter);

            OpenEndPoints.Add("Start");
            OpenEndPoints.Add("End");
        }
        public List<Control> getNeighbours(int x, int y, int range)
        {
            SortedDictionary<Double, Control> boxList = new SortedDictionary<Double, Control>();
            foreach (Control box in this.Controls)
            {
                if (box is PictureBox && box.Name != "pictureBox6")
                {
                    int boxX = box.Location.X + 25;
                    int boxY = box.Location.Y + 25;
                    double dist = Math.Sqrt(Math.Pow(boxX - x, 2) + Math.Pow(boxY - y, 2)) + rng.NextDouble();
                    if (dist < range)
                        boxList.Add(dist, box);
                }
            }
            List<Control> result = boxList.Values.ToArray().ToList();
            result.Add(pictureBox10);
            return result;
        }
        void mouseFilter_FormClicked(object sender, EventArgs e)
        {
            Point mousePos = this.PointToClient(Cursor.Position);
            Control selectedBox = getNeighbours(mousePos.X, mousePos.Y, 50)[0];

            if (selectedBox.BackColor == Color.Black)
                selectedBox.BackColor = Color.White;
            else
                selectedBox.BackColor = Color.Black;
        }
        void mouseFilter_RFormClicked(object sender, EventArgs e)
        {
            Point mousePos = this.PointToClient(Cursor.Position);
            setEndPoints(mousePos.X, mousePos.Y);
        }

        public void setTiles(Control body)
        {
            if (body == end)
            {
                isEnd = true;
            }
            else
            {
                double cost = double.Parse(body.Text);
                visitedTiles.Add(body);
                Point pos = body.Location;
                Control[] neighbours = getNeighbours(pos.X + 25, pos.Y + 25, 60).ToArray()[1..];
                foreach (Control box in neighbours)
                {
                    if (box.BackColor == Color.Black || visitedTiles.Contains(box))
                    {
                        activeTiles.Remove(box);
                        visitedTiles.Add(box);
                    }
                    else if (!activeTiles.Contains(box))
                    {
                        box.Text = (cost + 1 + rng.NextDouble()).ToString();
                        activeTiles.Add(box);
                        visitedTiles.Remove(box);
                    }
                }
            }
        }
        public void setEndPoints(int x, int y)
        {
            Control SelectedBox = getNeighbours(x, y, 50)[0];
            if (SelectedBox.BackColor == Color.White && OpenEndPoints.Count > 0)
            {
                if (OpenEndPoints[0] == "Start")
                {
                    SelectedBox.BackColor = Color.Blue;
                    start = SelectedBox;
                    start.Text = "0";
                    OpenEndPoints.Remove("Start");
                }
                else
                {
                    SelectedBox.BackColor = Color.Red;
                    end = SelectedBox;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            setTiles(start);
            while (!isEnd)
            {
                int range = activeTiles.Count();
                for (int x = 0; x < range; x++)
                {
                    setTiles(activeTiles[0]);
                    activeTiles.RemoveAt(0);
                }
            }
            retrace(end.Location);
            start.BackColor = Color.Blue;
            label1.Text = $"Cost: {totalcost}";
            label2.Text = $"Path Length: {50 * totalcost}px";

        }

        private void retrace(Point pos)
        {
            List<Control> ancestors = getNeighbours(pos.X + 25, pos.Y + 25, 60);
            SortedDictionary<double, Control> weightedBoxes = new SortedDictionary<double, Control>();
            foreach (Control box in ancestors)
            {
                if (box.BackColor != Color.Black)
                {

                    weightedBoxes.Add(double.Parse(box.Text), box);
                }
            }
            Control nextBox = weightedBoxes.Values.ToArray()[0];
            nextBox.BackColor = Color.Plum;
            totalcost++;
            if (nextBox != start)
                retrace(nextBox.Location);
        }
    }
}