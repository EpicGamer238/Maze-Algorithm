/*HEADER
 * Author: Simon Wunderlich
 * For VCE Units 1 & 2 Computing
 * Date of last edit: 22/04/23
 * Summary: A program that allows users to draw out a maze on an 8x8 grid. The program can then solve the maze and provide the most efficient route
 */

/*PSEUDOCODE
 * START
 * 
 * DEFINE tile_Clicked(object sender, EventArgs e)
 *      IF (sender colour = white) THEN
 *          sender colour <- black
 *      ELSE
 *          sender colour <- white
 *  
 *  DEFINE tile_RightClicked(object sender, EventArgs e)
 *      IF (startTile = null) THEN
 *          startTile <- sender
 *          sender colour <- blue
 *      ELSE IF (endTile = null) THEN
 *          endTIle <- sender
 *          sender colour <- red
 * 
 *  DEFINE getNeighbours(Tile tile)
 *      LIST neighbours
 *      FOR x <- COUNT tileAmt
 *          IF tileList[x] distance < 50 THEN
 *              neightbours ADD tileList[x]
 *      RETRUN neighbours
 *  DEFINE setNeighbours(Tile tile)
 *      invalidTiles ADD tile
 *      LIST neighbours = CALL getNeighbours(tile)
 *      FOR x<- COUNT neighbours length
 *          if neighbour[x] <> valid and <> black and <> invalid THEN
 *              LIST validTiles ADD neighbours[x]
 *              neighbours[x] Cost <- tile Cost PLUS 1
 *          ELSE IF neighbour <> invalid
 *              LIST invalidTiles ADD neighbours[x]
 *              
 *   DEFINE retrace(Tile tile)
 *      IF tile <> startTile THEN
 *          tile color <- grey
 *          LIST neighCostList
 *          neighbours <- getNeighbours(tile)
 *          FOR x <- COUNT neighbours LENGTH
 *              neighCostList ADD (neighbours[x] COST, neighbours[x])
 *          SORT neighCostList
 *          retrace(neighCostList[0]
 *  IF 'solve' button pressed THEN
 *      setNeighbours(startTile)
 *      
 *      WHILE validTiles does not contain endTile
 *          setNeighbours(validTiles[0])
 *          validTiles REMOVE AT INDEX 0
 *      retrace(end)
 *      
 */
using System.Runtime.InteropServices;

namespace TEST
{
    public partial class Form1 : Form
    {
        List<Control> validTiles = new List<Control>();
        List<Control> invalidTiles = new List<Control>();
        List<String> OpenEndPoints = new List<string>();
        Random rng = new Random();
        Control start;
        Control end;
        bool isEnd = false;
        bool Display = false;
        int totalcost = 1;
        public Form1()
        {
            InitializeComponent();

            //Initialises the methods to FormClicked and RFormClicked
            MouseDownFilter mouseFilter = new MouseDownFilter(this);
            mouseFilter.FormClicked += mouseFilter_FormClicked;
            mouseFilter.RFormClicked += mouseFilter_RFormClicked;
            Application.AddMessageFilter(mouseFilter);

            //A list of the available endpoints that haven't been placed yet
            OpenEndPoints.Add("Start");
            OpenEndPoints.Add("End");
        }
        //Given a position on the canvas and a range, it will find every box within the range of the position
        public List<Control> getNeighbours(int x, int y, int range)
        {
            SortedDictionary<Double, Control> boxList = new SortedDictionary<Double, Control>();

            //Iterates through every component in the design
            foreach (Control box in this.Controls)
            {
                //Checks that the current box is a PictureBox 
                if (box is PictureBox)
                {
                    //Offsets the box's position to be centred
                    int boxX = box.Location.X + 25;
                    int boxY = box.Location.Y + 25;

                    //Finds distance between the middle of the box and the provided coordinates
                    double dist = Math.Sqrt(Math.Pow(boxX - x, 2) + Math.Pow(boxY - y, 2)) + rng.NextDouble();

                    //If the distance is within range, the box and it's distance is added to a sorted dictionary
                    if (dist < range)
                        boxList.Add(dist, box);
                }
            }
            //Gets a list of all boxes within range ordered from closest to furthest
            List<Control> result = boxList.Values.ToArray().ToList();

            //An extra picture box is added to if there are no boxes within range so an error isnt thrown. Picturebox10 is hidden so it has no impact on the program
            if (result.Count == 0)
                result.Add(pictureBox10);

            return result;
        }

        //Function that is called whenever the mouse is clicked anywhere on the form
        //Gets closest box to the mouse's position and toggles it between black and white
        void mouseFilter_FormClicked(object sender, EventArgs e)
        {
            //Gets mouse position relative to the form window
            Point mousePos = this.PointToClient(Cursor.Position);

            //Finds the closest box to the mouse cursor
            Control selectedBox = getNeighbours(mousePos.X, mousePos.Y, 50)[0];

            //If box is black, make it white
            //Otherwise, make it black
            if (selectedBox.BackColor == Color.Black)
                selectedBox.BackColor = Color.White;
            else
            {
                selectedBox.BackColor = Color.Black;

                //If box was the start point, add 'start' to the list of open end points
                if (selectedBox == start)
                {
                    OpenEndPoints.Add("Start");
                }
            }
            //If the maze is displaying the result and the form is clicked, the path is reset back to white and the info is hidden
            if (Display)
            {
                Display = false;
                foreach (Control tile in this.Controls)
                {
                    if (tile.BackColor == Color.Plum)
                    {
                        tile.BackColor = Color.White;
                    }
                }
                label1.Visible = false;
                label2.Visible = false;
                panel1.Visible = false;
            }
        }

        //Function places endpoints for the program to start and finish at
        public void setEndPoints(int x, int y)
        {
            //Gets closest box to given coords
            Control SelectedBox = getNeighbours(x, y, 50)[0];

            //checks if the closest box is white and not all endpoints have been placed
            if (SelectedBox.BackColor == Color.White && OpenEndPoints.Count > 0)
            {
                //If start point hasn't been placed, place it
                if (OpenEndPoints.Contains("Start"))
                {
                    //Turns the square blue and sets the start object to the selected box
                    SelectedBox.BackColor = Color.Blue;
                    start = SelectedBox;

                    //Sets starting cost to 0
                    start.Text = "0";

                    //Removes start from the openendpoints list as it is no longer available to place down
                    OpenEndPoints.Remove("Start");
                }
                //If start point has been placed, place the finish point
                else
                {
                    //Turns the square red and sets the end object to the selected box
                    SelectedBox.BackColor = Color.Red;
                    end = SelectedBox;
                }
            }
        }

        //When the right mouse button is clicked any where on the form, place an endpoint at the nearest square
        void mouseFilter_RFormClicked(object sender, EventArgs e)
        {
            //Gets relative cursor position
            Point mousePos = this.PointToClient(Cursor.Position);

            //Calls setEndPoints with the cursors location
            setEndPoints(mousePos.X, mousePos.Y);
        }

        //Finds the adjacent squares and determines whether they are valid to continue the path
        public void setTiles(Control body)
        {
            //Checks if the algorithm has reached the endpoint
            if (body == end)
            {
                isEnd = true;
            }
            else
            {
                //Cost is the amount of tiles it has taken to get to the current position
                //As a PictureBox has no inherent cost value in the object, 'text' is used to store cost
                double cost = double.Parse(body.Text);

                //Labels the current square as invalid and no longer valid
                invalidTiles.Add(body);
                Point pos = body.Location;

                //Gets the directly adjacent squares, excluding the current square
                List<Control> neighbours = getNeighbours(pos.X + 25, pos.Y + 25, 80).ToArray()[1..].ToList();

                //Iterates through the neighbouring squares and labels them as either valid or invalid
                foreach (Control box in neighbours)
                {
                    //If the square is black or already marked as invalid, it is labeled as invalid
                    if (box.BackColor == Color.Black || invalidTiles.Contains(box))
                    {
                        validTiles.Remove(box);
                        invalidTiles.Add(box);
                    }
                    //If the square is not black and hasn't already been labelled as valid, then it is added to the valid list and assigned a cost
                    else if (!validTiles.Contains(box))
                    {
                        //Cost is determined by adding 1 to the 'body' squares cost
                        //A random decimal is also added such that if two squares have the same cost, they will not clash when retracing. Instead one will randomly be lower than the other and that path will be chosen
                        box.Text = (cost + 1 + rng.NextDouble()).ToString();

                        validTiles.Add(box);
                        invalidTiles.Remove(box);
                    }
                }
            }
        }
        //Function that retraces from the finish point and finds the most efficient path
        private void retrace(Point pos)
        {
            //Finds directly adjacent squares to current square
            List<Control> ancestors = getNeighbours(pos.X + 25, pos.Y + 25, 60).ToArray()[1..].ToList();

            //A sorted dictionary which will hold a square's cost and control, sorted by cost in acending order
            SortedDictionary<double, Control> weightedBoxes = new SortedDictionary<double, Control>();

            //Iterates through ancestors and finds white squares
            foreach (Control box in ancestors)
            {
                //Checks if square is white
                if (box.BackColor != Color.Black)
                {
                    //Adds the square and it's cost to weighted Boxes
                    weightedBoxes.Add(double.Parse(box.Text), box);
                }

            }
            //Finds the square with the lowest cost and assigns it to 'nextBox'
            Control nextBox = weightedBoxes.Values.ToArray()[0];
            //Marks the chosen square 
            nextBox.BackColor = Color.Green;
            totalcost++;

            //Repeats the function until it reaches the start of the path
            if (nextBox != start)
                retrace(nextBox.Location);
        }

        //When the 'SOLVE' button is clicked, it determines whether each tile is valid or invalid, then it retraces back from the end of the maze
        private void button1_Click(object sender, EventArgs e)
        {
            validTiles.Clear();
            invalidTiles.Clear();
            totalcost = 1;
            isEnd = false;
            //Determines whether start's surrounding tiles are valid
            setTiles(start);

            //Loops through the entire maze until it finds the end and determines the validity of each tile it selectes
            while (!isEnd)
            {
                setTiles(validTiles[0]);
                validTiles.RemoveAt(0);
            }

            //Retraces back through maze from end point
            retrace(end.Location);
            start.BackColor = Color.Blue;

            //Displays path info
            label1.Visible = true;
            label2.Visible = true;
            panel1.Visible = true;
            label1.Text = $"Cost: {totalcost}";
            label2.Text = $"Path Length: {50 * totalcost}px";
            Display = true;
        }
        //When 'RESET' button is pressed, every public variable is reset and every maze tile is set back to black
        private void button2_Click(object sender, EventArgs e)
        {
            //Sets every public variable back to the default values
            validTiles.Clear();
            invalidTiles.Clear();
            totalcost = 1;
            isEnd = false;
            OpenEndPoints.Add("Start");
            OpenEndPoints.Add("End");

            //Gets every picture box turns it black
            foreach (Control tile in this.Controls)
            {
                //Checks if the control is a picture box and changes its colour to black
                if (tile is PictureBox)
                {
                    tile.BackColor = Color.Black;
                }
            }
        }
    }
}