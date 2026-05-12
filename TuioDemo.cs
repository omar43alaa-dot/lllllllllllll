/*
	TUIO C# Demo - part of the reacTIVision project
	Copyright (c) 2005-2016 Martin Kaltenbrunner <martin@tuio.org>

	This program is free software; you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using TUIO;
using System.IO;
using WindowsFormsApp21;

public class TuioDemo2 : Form , TuioListener
	{
		private TuioClient client;
		private Dictionary<long,TuioObject> objectList;
		private Dictionary<long,TuioCursor> cursorList;
		private Dictionary<long,TuioBlob> blobList;

		public static int width, height;
		private int window_width =  640;
		private int window_height = 480;
		private int window_left = 0;
		private int window_top = 0;
		private int screen_width = Screen.PrimaryScreen.Bounds.Width;
		private int screen_height = Screen.PrimaryScreen.Bounds.Height;

		private bool fullscreen;
		private bool verbose;
    private bool animalInCageState = false;
    private bool food = false;
    private string selectedFoodImage = "";
    private string selectedhabitateImage = "";
    private string selectednameImage = "";
    private string pendingMenuItem = "";
    private string confirmedMenuItem = "";
    private string selectedFactText = "";
    private DateTime menuItemHoverStart = DateTime.MinValue;
    private string currentUser;
    private string customAnimal = "default";
    private string customBackground = "Zoo.jpg";
    private string customAsset = "default";
    private string userGender = "male";
    private bool isUpdating = false;
    private enum ControlMode
    {
        Animal,
        Menu,
        Quiz
    }
    private ControlMode currentMode = ControlMode.Animal;
    private DateTime hoverStartTime = DateTime.MinValue;
    private string hoverTarget = "";
    private string selectedMenuItem = "";
    Font font = new Font("Arial", 10.0f);
		SolidBrush fntBrush = new SolidBrush(Color.White);
		SolidBrush bgrBrush = new SolidBrush(Color.FromArgb(0,0,64));
		SolidBrush curBrush = new SolidBrush(Color.FromArgb(192, 0, 192));
		SolidBrush objBrush = new SolidBrush(Color.FromArgb(64, 0, 0));
		SolidBrush blbBrush = new SolidBrush(Color.FromArgb(64, 64, 64));
		Pen curPen = new Pen(new SolidBrush(Color.Blue), 1);
    class Question
    {
        public string Text;
        public string[] Options;
        public int correctindex;
    }

    private Dictionary<int, Question> markerQuestions = new Dictionary<int, Question>()
{
    { 0, new Question { Text = "Where do crocodiles live?", Options = new string[] { "Forests", "Grasslands", "Water", "Farm" }, correctindex = 2 } },
    { 1, new Question { Text = "What does a lion eat?", Options = new string[] { "Grass", "Meat", "Fish", "Leaves" }, correctindex = 1 } },
    { 2, new Question { Text = "Where does a monkey live?", Options = new string[] { "Ocean", "Tree", "Desert", "Ice" }, correctindex = 1 } },
    { 3, new Question { Text = "What do birds eat?", Options = new string[] { "Rocks", "Meat", "Metal", "Seeds" }, correctindex = 3 } }
};

    private Dictionary<int, bool> answeredMarkers = new Dictionary<int, bool>();

    int hoveredOption = -1;
    DateTime hoverStart = DateTime.MinValue;
    string answerFeedback = "";
    DateTime feedbackStart = DateTime.MinValue;
    bool showFeedback = false;
    int score = 0;
    bool quizFinished = false;
    DateTime quizFinishedTime = DateTime.MinValue;
    private string GetMenuAction(float angle)
    {
        string[] items = { "Food", "Habitat", "Fact", "Name", "Quiz", "Update", "Logout", "Home" };

        double deg = angle * 180.0 / Math.PI;
        if (deg < 0)
            deg += 360.0;

        double sectorSize = 360.0 / items.Length;   // 51.428...
        int index = (int)(deg / sectorSize);

        if (index >= items.Length)
            index = items.Length - 1;

        return items[index];
    }
    private bool IsPointInsideRect(int px, int py, Rectangle rect)
    {
        return rect.Contains(px, py);
    }
    private void UpdateHoverControl(int markerX, int markerY, Rectangle menuRect, Rectangle cageRect)
    {
        string newHoverTarget = "";

        if (IsPointInsideRect(markerX, markerY, menuRect))
        {
            newHoverTarget = "menu";
        }
        else if (IsPointInsideRect(markerX, markerY, cageRect))
        {
            newHoverTarget = "animal";
        }

        if (newHoverTarget != hoverTarget)
        {
            hoverTarget = newHoverTarget;
            hoverStartTime = DateTime.Now;
            return;
        }

        if (hoverTarget == "") return;

        TimeSpan hoverDuration = DateTime.Now - hoverStartTime;

        if (hoverDuration.TotalSeconds >= 5)
        {
            if (hoverTarget == "menu")
                currentMode = ControlMode.Menu;
            else if (hoverTarget == "animal")
                currentMode = ControlMode.Animal;
        }
    }
    private void UpdateMenuSelectionHold(string currentItem)
    {
        if (currentMode != ControlMode.Menu)
        {
            pendingMenuItem = "";
        
            menuItemHoverStart = DateTime.MinValue;
            return;
        }

        if (currentItem != pendingMenuItem)
        {
            pendingMenuItem = currentItem;
            menuItemHoverStart = DateTime.Now;
            return;
        }

        if (pendingMenuItem == "") return;

        TimeSpan holdDuration = DateTime.Now - menuItemHoverStart;
        
        int threshold = (pendingMenuItem == "Update") ? 3 : 8;

        if (holdDuration.TotalSeconds >= threshold)
        {
            confirmedMenuItem = pendingMenuItem;
        }
    }
    private void DrawCircularMenu(Graphics g, int x, int y, string selectedItem)
    {
        string[] items = { "Food", "Habitat", "Fact", "Name", "Quiz", "Update", "Logout", "Home" };
        int radius = 70;
        double step = 360.0 / items.Length;

        for (int i = 0; i < items.Length; i++)
        {
            double angle = (i * step - 90) * Math.PI / 180.0;

            int mx = x + (int)(radius * Math.Cos(angle));
            int my = y + (int)(radius * Math.Sin(angle));

            Brush fillBrush = (items[i] == selectedItem) ? Brushes.HotPink : Brushes.DarkSlateBlue;
            Pen borderPen = (items[i] == selectedItem) ? new Pen(Color.Yellow, 3) : Pens.White;

            g.FillEllipse(fillBrush, mx - 28, my - 28, 56, 56);
            g.DrawEllipse(borderPen, mx - 28, my - 28, 56, 56);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            g.DrawString(items[i], new Font("Arial", 7, FontStyle.Bold), Brushes.White,
                new RectangleF(mx - 25, my - 15, 50, 30), sf);
        }

        g.FillEllipse(Brushes.DarkSlateBlue, x - 35, y - 35, 70, 70);
        g.DrawEllipse(new Pen(Color.White, 3), x - 35, y - 35, 70, 70);

        StringFormat centerSf = new StringFormat();
        centerSf.Alignment = StringAlignment.Center;
        centerSf.LineAlignment = StringAlignment.Center;

        g.DrawString("Menu", new Font("Arial", 9, FontStyle.Bold), Brushes.White,
            new RectangleF(x - 30, y - 15, 60, 30), centerSf);
    }
    public TuioDemo2(int port, String username, string animal = "default", string bg = "Zoo.jpg", string asset = "default", string gender = "male") {
        currentUser = username; 
        customAnimal = animal;
        customBackground = bg;
        customAsset = asset;
        userGender = gender;

        verbose = false;
			fullscreen = false;
			width = window_width;
			height = window_height;

			this.ClientSize = new System.Drawing.Size(width, height);
			this.Name = "TuioDemo";
			this.Text = "TuioDemo";
			
			this.Closing+=new CancelEventHandler(Form_Closing);
			this.KeyDown+=new KeyEventHandler(Form_KeyDown);

			this.SetStyle( ControlStyles.AllPaintingInWmPaint |
							ControlStyles.UserPaint |
							ControlStyles.DoubleBuffer, true);

			objectList = new Dictionary<long,TuioObject>(128);
			cursorList = new Dictionary<long,TuioCursor>(128);
			blobList   = new Dictionary<long,TuioBlob>(128);
			
			client = new TuioClient(port);
			client.addTuioListener(this);

			client.connect();

            // Subscribe to emotion updates
            EmotionSocketClient.OnEmotionReceived += (emotion) => {
                if (this.IsDisposed || !this.IsHandleCreated) return;
                this.BeginInvoke(new Action(() => this.Invalidate()));
            };
		}

		private void Form_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {

 			if ( e.KeyData == Keys.F1) {
	 			if (fullscreen == false) {

					width = screen_width;
					height = screen_height;

					window_left = this.Left;
					window_top = this.Top;

					this.FormBorderStyle = FormBorderStyle.None;
		 			this.Left = 0;
		 			this.Top = 0;
		 			this.Width = screen_width;
		 			this.Height = screen_height;

		 			fullscreen = true;
	 			} else {

					width = window_width;
					height = window_height;

		 			this.FormBorderStyle = FormBorderStyle.Sizable;
		 			this.Left = window_left;
		 			this.Top = window_top;
		 			this.Width = window_width;
		 			this.Height = window_height;

		 			fullscreen = false;
	 			}
 			} else if ( e.KeyData == Keys.Escape) {
				this.Close();

 			} else if ( e.KeyData == Keys.V ) {
 				verbose=!verbose;
 			}

 		}

		private void Form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			client.removeTuioListener(this);

			client.disconnect();
			if (!isUpdating) System.Environment.Exit(0);
		}

		public void addTuioObject(TuioObject o) {
			lock(objectList) {
				objectList.Add(o.SessionID,o);
			} if (verbose) Console.WriteLine("add obj "+o.SymbolID+" ("+o.SessionID+") "+o.X+" "+o.Y+" "+o.Angle);
		}

		public void updateTuioObject(TuioObject o) {

			if (verbose) Console.WriteLine("set obj "+o.SymbolID+" "+o.SessionID+" "+o.X+" "+o.Y+" "+o.Angle+" "+o.MotionSpeed+" "+o.RotationSpeed+" "+o.MotionAccel+" "+o.RotationAccel);
		}

		public void removeTuioObject(TuioObject o) {
			lock(objectList) {
				objectList.Remove(o.SessionID);
			}
			if (verbose) Console.WriteLine("del obj "+o.SymbolID+" ("+o.SessionID+")");
		}

		public void addTuioCursor(TuioCursor c) {
			lock(cursorList) {
				cursorList.Add(c.SessionID,c);
			}
			if (verbose) Console.WriteLine("add cur "+c.CursorID + " ("+c.SessionID+") "+c.X+" "+c.Y);
		}

		public void updateTuioCursor(TuioCursor c) {
			if (verbose) Console.WriteLine("set cur "+c.CursorID + " ("+c.SessionID+") "+c.X+" "+c.Y+" "+c.MotionSpeed+" "+c.MotionAccel);
		}

		public void removeTuioCursor(TuioCursor c) {
			lock(cursorList) {
				cursorList.Remove(c.SessionID);
			}
			if (verbose) Console.WriteLine("del cur "+c.CursorID + " ("+c.SessionID+")");
 		}

		public void addTuioBlob(TuioBlob b) {
			lock(blobList) {
				blobList.Add(b.SessionID,b);
			}
			if (verbose) Console.WriteLine("add blb "+b.BlobID + " ("+b.SessionID+") "+b.X+" "+b.Y+" "+b.Angle+" "+b.Width+" "+b.Height+" "+b.Area);
		}

		public void updateTuioBlob(TuioBlob b) {
		
			if (verbose) Console.WriteLine("set blb "+b.BlobID + " ("+b.SessionID+") "+b.X+" "+b.Y+" "+b.Angle+" "+b.Width+" "+b.Height+" "+b.Area+" "+b.MotionSpeed+" "+b.RotationSpeed+" "+b.MotionAccel+" "+b.RotationAccel);
		}

		public void removeTuioBlob(TuioBlob b) {
			lock(blobList) {
				blobList.Remove(b.SessionID);
			}
			if (verbose) Console.WriteLine("del blb "+b.BlobID + " ("+b.SessionID+")");
		}

		public void refresh(TuioTime frameTime) {
			Invalidate();
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			
			Graphics g = pevent.Graphics;
            
            // Gender-based background color fallback
             Color bgColor = userGender.Equals("female", StringComparison.OrdinalIgnoreCase) ? Color.MistyRose : Color.FromArgb(0, 0, 64);
			g.FillRectangle(new SolidBrush(bgColor), new Rectangle(0,0,width,height));

            // Draw current facial expression feedback
            string currentEmotion = EmotionSocketClient.CurrentEmotion;
            string feedbackText = "";
            Brush textBrush = Brushes.Gold;

            if (currentEmotion == "Happy")
            {
                feedbackText = "im so glad that you happy";
                textBrush = Brushes.LimeGreen;
            }
            else if (currentEmotion == "Sad")
            {
                feedbackText = "be calm and solve be calm";
                textBrush = Brushes.OrangeRed;
            }

            if (!string.IsNullOrEmpty(feedbackText))
            {
                g.DrawString(feedbackText, new Font("Arial", 20, FontStyle.Bold), textBrush, new PointF(20, 20));
            }

             int menuCenterX = width - 130;
             int menuCenterY = height - 130;
             Rectangle menuRect = new Rectangle(menuCenterX - 110, menuCenterY - 110, 220, 220);
             
             if (cursorList.Count > 0) 
        {
 			 lock(cursorList) {
			 foreach (TuioCursor tcur in cursorList.Values) {
					List<TuioPoint> path = tcur.Path;
					TuioPoint current_point = path[0];

					for (int i = 0; i < path.Count; i++) {
						TuioPoint next_point = path[i];
						g.DrawLine(curPen, current_point.getScreenX(width), current_point.getScreenY(height), next_point.getScreenX(width), next_point.getScreenY(height));
						current_point = next_point;
					}
					g.FillEllipse(curBrush, current_point.getScreenX(width) - height / 100, current_point.getScreenY(height) - height / 100, height / 50, height / 50);
					g.DrawString(tcur.CursorID + "", font, fntBrush, new PointF(tcur.getScreenX(width) - 10, tcur.getScreenY(height) - 10));
				}
			}
		 }

			if (objectList.Count > 0) {
                if (File.Exists(customBackground))
                {
                    Bitmap imgBg = new Bitmap(customBackground);
                    g.DrawImage(imgBg, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
                }
 				lock(objectList) {

					foreach (TuioObject tobj in objectList.Values) {
					if (tobj.SymbolID == 0)
					{
                        double angle = tobj.Angle;
                        bool isCageOpen = (angle > 1.0 && angle < 3.5);
                    }
					else if (tobj.SymbolID == 1)
					{
                    }
                    else if (tobj.SymbolID == 2)
                    {
                    }
					else if (tobj.SymbolID == 3)
					{
                    }
                    int ox = tobj.getScreenX(width);
                    int oy = tobj.getScreenY(height);
                    int size = height / 10;
                    double anglee = tobj.Angle;

                    if (currentMode == ControlMode.Animal)
                    {
                        animalInCageState = (anglee > 1.0 && anglee < 3.5);
                    }

                    bool isAnimalInCage = animalInCageState;
                    string animalOnlyPath = "";
                    string animalInCagePath = "";
                    string cageOnlyPath = "";
                    string currentFoodImage = "";
                    string currenthabitateImage = "";
                    string currentnameImage = "";
                    string currentFactText = "";
                    int cageX = 0;
                    int cageY = 0;
                    int animalW = 100;
                    int animalH = 100;
                    int cageW = 150;
                    int cageH = 150;
                    if (tobj.SymbolID == 0)
                    {
                        animalOnlyPath = "a3.jpeg";
                        animalInCagePath = "a2.jpeg";
                        cageOnlyPath = "a1.jpeg";
                        cageX = 50;
                        cageY = 250;
                        animalW = 100;
                        animalH = 100;
                        cageW = 150;
                        cageH = 150;
                        currentFoodImage = "f1.jpeg";
                        currenthabitateImage = "h1.jpeg";
                        currentnameImage = "n1.png";
                        currentFactText = "Crocodiles live in water and on land.\r\nThey have very strong jaws and sharp teeth.\r\nCrocodiles can stay still for a long time.\r\nThey are excellent swimmers.";
                    }
                    else if (tobj.SymbolID == 1)
                    {
                        animalOnlyPath = "b1.jpeg";
                        animalInCagePath = "b2.jpeg";
                        cageOnlyPath = "b3.jpeg";
                        cageX = 50;
                        cageY = 250;
                        animalW = 100;
                        animalH = 100;
                        cageW = 150;
                        cageH = 150;
                        currentFoodImage = "f2.jpeg";
                        currentnameImage = "n2.png";
                        currenthabitateImage = "h2.jpeg";
                        currentFactText = "Lions are called the king of the jungle.\r\nLions live in groups called prides.\r\nMale lions have big hair called a mane.\r\nLions are very strong hunters.";

                    }
                    else if (tobj.SymbolID == 2)
                    {
                        animalOnlyPath = "m1.jpeg";
                        animalInCagePath = "m2.jpeg";
                        cageOnlyPath = "m3.jpeg";
                        cageX = 50;
                        cageY = 250;
                        animalW = 120;
                        animalH = 120;
                        cageW = 170;
                        cageH = 170;
                        currentFoodImage = "f3.jpeg";
                        currenthabitateImage = "h3.jpeg";
                        currentnameImage = "n3.png";
                        currentFactText = "Monkeys love to climb trees.\r\nThey like to eat bananas and fruits.\r\nMonkeys use their tails to balance.\r\nThey are very smart and playful.";
                    }
                    else if (tobj.SymbolID == 3)
                    {
                        animalOnlyPath = "4.png";
                        animalInCagePath = "cage.jpeg";
                        cageOnlyPath = "cage.png";
                        cageX = 50;
                        cageY = 250;
                        animalW = 120;
                        animalH = 120;
                        cageW = 170;
                        cageH = 170;
                        currenthabitateImage = "h4.jpeg";
                        currentFoodImage = "f4.jpeg";
                        currentnameImage = "n4.png";
                        currentFactText = "Birds have feathers and wings.\r\nMost birds can fly in the sky.\r\nBirds build nests to lay eggs.\r\nThey eat seeds, worms, or insects.";

                    }
                    else if (tobj.SymbolID == 5)
                    {
                        animalOnlyPath = (customAnimal != "default") ? customAnimal : "b1.jpeg";
                        animalInCagePath = animalOnlyPath;
                        cageOnlyPath = (customAsset != "default") ? customAsset : "cage.png";
                        cageX = 50;
                        cageY = 250;
                        animalW = 150;
                        animalH = 150;
                        cageW = 200;
                        cageH = 200;

                        // Default to Lion data if unknown
                        currentFoodImage = "f2.jpeg";
                        currenthabitateImage = "h2.jpeg";
                        currentnameImage = "n2.png";
                        currentFactText = "This is your custom favorite animal!";

                        // Map data based on the filename of the custom animal
                        if (animalOnlyPath.Contains("a1") || animalOnlyPath.Contains("a2") || animalOnlyPath.Contains("a3")) {
                            currentFoodImage = "f1.jpeg";
                            currenthabitateImage = "h1.jpeg";
                            currentnameImage = "n1.png";
                            currentFactText = "Crocodiles live in water and on land.\r\nThey have very strong jaws and sharp teeth.";
                        }
                        else if (animalOnlyPath.Contains("b1") || animalOnlyPath.Contains("b2") || animalOnlyPath.Contains("b3")) {
                            currentFoodImage = "f2.jpeg";
                            currenthabitateImage = "h2.jpeg";
                            currentnameImage = "n2.png";
                            if (userGender == "female")
                                currentFactText = "Lionesses are the main hunters of the pride.\r\nThey work together to protect their cubs.";
                            else
                                currentFactText = "Lions are called the king of the jungle.\r\nMale lions have thick manes to protect their necks.";
                        }
                        else if (animalOnlyPath.Contains("m1") || animalOnlyPath.Contains("m2") || animalOnlyPath.Contains("m3")) {
                            currentFoodImage = "f3.jpeg";
                            currenthabitateImage = "h3.jpeg";
                            currentnameImage = "n3.png";
                            currentFactText = "Monkeys love to climb trees.\r\nThey like to eat bananas and fruits.";
                        }
                        else if (animalOnlyPath.Contains("4")) {
                            currentFoodImage = "f4.jpeg";
                            currenthabitateImage = "h4.jpeg";
                            currentnameImage = "n4.png";
                            currentFactText = "Birds have feathers and wings.\r\nMost birds can fly in the sky.";
                        }

                        // Override habitat image if a custom background was selected
                        if (customBackground != "zoo.jpg" && customBackground != "Zoo.jpg") {
                            currenthabitateImage = customBackground;
                        }
                    }
                    if (confirmedMenuItem == "Food")
                    {
                        selectedFoodImage = currentFoodImage;
                    }
                    if (confirmedMenuItem == "Habitat")
                    {
                        selectedhabitateImage = currenthabitateImage;
                    }
                    if (confirmedMenuItem == "Name")
                    {
                        selectednameImage = currentnameImage;
                    }
                    if (confirmedMenuItem == "Fact")
                    {
                        selectedFactText = currentFactText;
                    }
                    if (confirmedMenuItem == "Update")
                    {
                        isUpdating = true;
                        this.Close();
                        CustomizationForm customForm = new CustomizationForm(currentUser);
                        customForm.Show();
                        confirmedMenuItem = ""; 
                    }
                    if (currentMode == ControlMode.Quiz)
                    {
                        DrawQuiz(g, tobj, cageX, cageY, cageW, cageH);

                        if (quizFinished && (DateTime.Now - quizFinishedTime).TotalSeconds >= 2)
                        {
                            currentMode = ControlMode.Animal;

                            answeredMarkers.Clear();
                            hoveredOption = -1;
                            answerFeedback = "";
                            showFeedback = false;
                            score = 0;

                            quizFinished = false;
                            confirmedMenuItem = "";
                            pendingMenuItem = "";
                            selectedMenuItem = "";
                        }

                        continue;
                    }
                    if (confirmedMenuItem == "Home")
                    {
                        currentMode = ControlMode.Animal;
                    }
                    Rectangle cageRect = new Rectangle(cageX, cageY, cageW, cageH);


                    UpdateHoverControl(ox, oy, menuRect, cageRect);

                    if (cageOnlyPath != "")
                    {
                        string cageToUse = (customAsset != "default" && customAsset != "cage.png") ? customAsset : cageOnlyPath;
                        if (File.Exists(cageToUse))
                        {
                            Bitmap cageImg = new Bitmap(cageToUse);
                            cageImg.MakeTransparent(Color.White);
                            g.DrawImage(cageImg, cageX, cageY, cageW, cageH);
                        }
                    }

                    if (animalOnlyPath != "" && isAnimalInCage)
                    {
                        Bitmap animalInCageImg = new Bitmap(animalInCagePath);
                        animalInCageImg.MakeTransparent(Color.White);
                        g.DrawImage(animalInCageImg, cageX, cageY, cageW, cageH);
                    }
                    if (currentMode == ControlMode.Menu)
                    {
                        selectedMenuItem = GetMenuAction((float)tobj.Angle);
                        UpdateMenuSelectionHold(selectedMenuItem);

                        if (confirmedMenuItem == "Quiz")
                        {
                            currentMode = ControlMode.Quiz;
                        }
                    }
                    else
                    {
                        selectedMenuItem = "";
                        pendingMenuItem = "";
                    }

                    if (animalOnlyPath != "" && !isAnimalInCage && currentMode == ControlMode.Animal)
                    {
                        g.TranslateTransform(ox, oy);
                        g.RotateTransform((float)(tobj.Angle / Math.PI * 180.0f));
                        g.TranslateTransform(-ox, -oy);
                        Bitmap animalOnlyImg = new Bitmap(animalOnlyPath);
                        animalOnlyImg.MakeTransparent(Color.White);
                        g.DrawImage(animalOnlyImg, ox, oy, animalW, animalH);
                        g.TranslateTransform(ox, oy);
                        g.RotateTransform(-1 * (float)(tobj.Angle / Math.PI * 180.0f));
                        g.TranslateTransform(-ox, -oy);
                    }
                    else if (animalOnlyPath == "")
                    {
                        g.FillRectangle(objBrush, new Rectangle(ox - size / 2, oy - size / 2, size, size));
                    }

                    g.DrawString(tobj.SymbolID + "", font, fntBrush, new PointF(ox - 10, oy - 10));
                }

				}
            
        }
        

        // draw the blobs
        if (blobList.Count > 0)
		{
				lock(blobList) {
					foreach (TuioBlob tblb in blobList.Values) {
						int bx = tblb.getScreenX(width);
						int by = tblb.getScreenY(height);
						float bw = tblb.Width*width;
						float bh = tblb.Height*height;

						g.TranslateTransform(bx, by);
						g.RotateTransform((float)(tblb.Angle / Math.PI * 180.0f));
						g.TranslateTransform(-bx, -by);

						g.FillEllipse(blbBrush, bx - bw / 2, by - bh / 2, bw, bh);

						g.TranslateTransform(bx, by);
						g.RotateTransform(-1 * (float)(tblb.Angle / Math.PI * 180.0f));
						g.TranslateTransform(-bx, -by);
						
						g.DrawString(tblb.BlobID + "", font, fntBrush, new PointF(bx, by));

					}
				}
        }
        DrawCircularMenu(g, width - 130, height - 130, selectedMenuItem);
        DrawSelectedMenuContent(g);
    }
    private void InitializeComponent()
    {
            this.SuspendLayout();
            // 
            // TuioDemo
            // 
            this.ClientSize = new System.Drawing.Size(292, 212);
            this.Name = "TuioDemo";
            this.Load += new System.EventHandler(this.TuioDemo_Load);
            this.ResumeLayout(false);

    }
    private void TuioDemo_Load(object sender, EventArgs e)
    {

    }
    private void DrawSelectedMenuContent(Graphics g)
    {
        Rectangle contentRect = new Rectangle(350, 40, 230, 170);
        if (confirmedMenuItem == "Food" || confirmedMenuItem == "Habitat" || confirmedMenuItem == "Name")
        {
            string imagePath = "";
            if (confirmedMenuItem == "Food")
                imagePath = selectedFoodImage;
            else if (confirmedMenuItem == "Habitat")
                imagePath = selectedhabitateImage;
            else if (confirmedMenuItem == "Name")
                imagePath = selectednameImage;

            if (imagePath != "")
            {
                Bitmap img = new Bitmap(imagePath);
                img.MakeTransparent(Color.White);
                g.DrawImage(img, contentRect);
            }
        }
        else if (confirmedMenuItem == "Fact")
        {
            g.FillRectangle(Brushes.Beige, contentRect);
            g.DrawRectangle(new Pen(Color.DarkBlue, 3), contentRect);

            string factText = selectedFactText;

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            g.DrawString(factText,
                new Font("Arial", 11, FontStyle.Bold),
                Brushes.Black,
                contentRect,
                sf);
        }
        else if (confirmedMenuItem == "logout")
        {
            Form1 pnn = new Form1();
            this.Hide();
            pnn.Show();
        }
        
    }
    private void UpdateUserScore()
    {
        string path = "users.txt";

        if (!File.Exists(path)) return;

        var lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("name")) continue;

            string[] parts = lines[i].Split(',');

            if (parts.Length >= 8)
            {
                string name = parts[0].Trim();

                if (name.Equals(currentUser, StringComparison.OrdinalIgnoreCase))
                {
                    int oldScore = 0;
                    int.TryParse(parts[7], out oldScore);

                    oldScore++; 

                    parts[7] = oldScore.ToString();

                    lines[i] = string.Join(",", parts);
                    break;
                }
            }
        }

        File.WriteAllLines(path, lines);
    }
    private void DrawQuiz(Graphics g, TuioObject tobj, int cageX, int cageY, int cageW, int cageH)
    {
        if (!markerQuestions.ContainsKey(tobj.SymbolID))
            return;

        Question q = markerQuestions[tobj.SymbolID];

        int optionX = cageX + 10;
        int optionStartY = cageY + 50;

        g.FillRectangle(Brushes.MidnightBlue, cageX, cageY, cageW, cageH);
        g.DrawRectangle(Pens.White, cageX, cageY, cageW, cageH);

        g.DrawString(q.Text,
            new Font("Arial", 11, FontStyle.Bold),
            Brushes.White,
            new RectangleF(cageX + 10, cageY + 10, cageW - 20, 35));

        for (int i = 0; i < q.Options.Length; i++)
        {
            int optionY = optionStartY + i * 35;
            Rectangle optionRect = new Rectangle(optionX, optionY, cageW - 20, 28);

            Brush brush = (i == hoveredOption) ? Brushes.Green : Brushes.DarkBlue;
            g.FillRectangle(brush, optionRect);
            g.DrawRectangle(Pens.White, optionRect);
            g.DrawString(q.Options[i], new Font("Arial", 9, FontStyle.Bold), Brushes.White,
                optionRect.X + 5, optionRect.Y + 5);
        }

        int newHovered = -1;
        for (int i = 0; i < q.Options.Length; i++)
        {
            Rectangle optionRect = new Rectangle(optionX, optionStartY + i * 35, cageW - 20, 28);
            if (optionRect.Contains(tobj.getScreenX(width), tobj.getScreenY(height)))
            {
                newHovered = i;
                break;
            }
        }

        if (newHovered != hoveredOption)
        {
            hoveredOption = newHovered;
            hoverStart = DateTime.Now;
        }

        if (hoveredOption != -1 && (DateTime.Now - hoverStart).TotalSeconds >= 2)
        {
            if (!answeredMarkers.ContainsKey(tobj.SymbolID))
            {
                if (hoveredOption == q.correctindex)
                {
                    score++;
                    UpdateUserScore();
                    answerFeedback = "Correct!";
                }
                else
                {
                    answerFeedback = "Wrong!";
                }

                answeredMarkers[tobj.SymbolID] = true;
                showFeedback = true;
                feedbackStart = DateTime.Now;
            }

            hoveredOption = -1;
        }
        if (answeredMarkers.Count == markerQuestions.Count && !quizFinished)
        {
            quizFinished = true;
            quizFinishedTime = DateTime.Now;
        }

        if (showFeedback)
        {
            if ((DateTime.Now - feedbackStart).TotalSeconds < 1.5)
            {
                g.DrawString(answerFeedback,
                    new Font("Arial", 14, FontStyle.Bold),
                    Brushes.Yellow,
                    cageX + 10, cageY + cageH - 30);
            }
            else
            {
                showFeedback = false;
            }
        }

        g.DrawString("Score: " + score,
            new Font("Arial", 10, FontStyle.Bold),
            Brushes.White,
            cageX + 10, cageY + cageH - 150);
        if (quizFinished)
        {
            g.FillRectangle(Brushes.DarkGreen, cageX + 10, cageY + cageH - 90, cageW - 20, 35);
            g.DrawRectangle(Pens.White, cageX + 10, cageY + cageH - 90, cageW - 20, 35);

            g.DrawString("Quiz Finished!", new Font("Arial", 11, FontStyle.Bold),
                Brushes.White, new RectangleF(cageX + 10, cageY + cageH - 90, cageW - 20, 35));
        }



    }
  //  public static void Main(String[] argv) {
	 //		int port = 0;
		//	switch (argv.Length) {
		//		case 1:
		//			port = int.Parse(argv[0],null); 
		//			if(port==0) goto default;
		//			break;
		//		case 0:
		//			port = 3333;
		//			break;
		//		default:
		//			Console.WriteLine("usage: mono TuioDemo [port]");
		//			System.Environment.Exit(0);
		//			break;
		//	}
			
		//	TuioDemo2 app = new TuioDemo2(port);
		//	Application.Run(app);
		//}
	}
