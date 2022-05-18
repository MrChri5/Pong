using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pong
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Cursor.Hide();
        }

        static int borderWidth = 15;
        static int ballSize = 20;
        static int playerSize = 100;
        char playerDirection = ' ';
        bool directionRight = true;        
        bool resetSwitch = true;
        bool holdResetSwitch = true;
        double ballGradient = 0;
        double ballYextra = 0;
        double ballXextra = 0;
        double ballSpeed = 0;
        double playerSpeed = 0;
        int playerScore = 0;
        int textFlash = 0;
        char drawMode = ' ';

        Point ballPoint = new Point(0,0);
        Point playerPoint = new Point(0,0);
        static SolidBrush ballColour = new SolidBrush(Color.White);
  
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            ResetGame();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //bounce off back wall
            if (ballPoint.X + ballSize >= panel1.Width - borderWidth)
            {
                directionRight = false;
                playerScore++;
                ballSpeed += 0.02;
                Random rndColour = new Random();
                //change to random colour
                Color[] colourList = new Color[10]
                    { Color.White , Color.Red , Color.Navy , Color.Green , Color.Yellow ,
                      Color.Orange , Color.Purple , Color.HotPink , Color.Lime , Color.Turquoise };
                ballColour.Color = colourList[rndColour.Next(0, colourList.Length)];  
                //if shallow gradient, speed up ball
                if (ballGradient <= 0.1 && ballGradient >= -0.1)
                {
                    ballSpeed += 0.2;
                }
                //if high score, slow down player
                if (playerScore > 10 && playerSpeed > 3)
                {
                    playerSpeed -= 0.1;
                }
            }

            //bounce off player paddle
            if (ballPoint.X <= borderWidth && ballPoint.Y + ballSize/2 <= playerPoint.Y + playerSize && ballPoint.Y + ballSize/2 >= playerPoint.Y && !holdResetSwitch)
            {
                directionRight = true;
                ballSpeed += 0.03;
                //add random change to gradient
                Random rndGrad = new Random();
                switch (playerDirection)
                {
                    case 'U':
                        ballGradient -= (double)rndGrad.Next(0, 50) / 100;
                        break;
                    case 'D':
                        ballGradient += (double)rndGrad.Next(0, 50) / 100;
                        break;
                    case ' ':
                        ballGradient += (double)rndGrad.Next(-10, 10) / 100;
                        break;
                }                   
            }
            //holdResetSwitch needed for previous check, so direction 
            //doesn't change when ball spawns on player paddle
            holdResetSwitch = resetSwitch;

            //GAME OVER
            if (ballPoint.X <= 0)
            {
                ResetGame();
                return;
            }

            //bounce off top or bottom wall
            if (ballPoint.Y <= borderWidth || ballPoint.Y + ballSize >= panel1.Height - borderWidth)
            {
                ballGradient = -ballGradient;
                ballSpeed += 0.01;
            }

            //calculate new ball position
            double holdYPos = 0;
            double holdXPos = 0;
            if (directionRight)
            {
                holdYPos = ballPoint.Y + ballYextra + ballGradient*ballSpeed;
                holdXPos = ballPoint.X + ballXextra + ballSpeed;
            }
            else
            {
                holdYPos = ballPoint.Y + ballYextra + ballGradient*ballSpeed;
                holdXPos = ballPoint.X + ballXextra - ballSpeed;
            }          
            ballPoint.Y = (int) Math.Round(holdYPos);
            ballPoint.X = (int) Math.Round(holdXPos);
            ballYextra = holdYPos - ballPoint.Y;
            ballXextra = holdXPos - ballPoint.X;

            //if ball outside boundaries, adjust position
            if(ballPoint.Y < borderWidth)
            {
                ballPoint.Y = borderWidth;
            }
            else if (ballPoint.Y > panel1.Height - borderWidth - ballSize)
            {
                ballPoint.Y = panel1.Height - borderWidth - ballSize;
            }
            if (ballPoint.X > panel1.Width - borderWidth - ballSize)
            {
                ballPoint.X = panel1.Width - borderWidth - ballSize;
            }

            //calculate new player position
            if (playerPoint.Y > borderWidth && playerDirection == 'U')
            {
                playerPoint.Y -= (int) playerSpeed;
            }
            if (playerPoint.Y < panel1.Height - playerSize - borderWidth && playerDirection == 'D')
            {
                playerPoint.Y += (int) playerSpeed;
            }

            drawMode = 'N';
            this.Refresh();            
        }


        //user inputs
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && resetSwitch)
            {
                timer1.Enabled = true;
                timer2.Enabled = false;
                resetSwitch = false;
                playerScore = 0;
            }
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
            {
                playerDirection = 'U';
            }
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
            {
                playerDirection = 'D';
            }
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        //reset key input on key release
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.W || e.KeyCode == Keys.S)
            {
                playerDirection = ' ';
            }
        }

        //reset game
        private void ResetGame()
        {          
            timer1.Enabled = false;
            timer2.Enabled = true;
            resetSwitch = holdResetSwitch = true;

            ballPoint.X = borderWidth;
            ballPoint.Y = (panel1.Height-ballSize) / 2;
            ballSpeed = 3.5;
            Random rndGrad = new Random();
            ballGradient = (double)rndGrad.Next(-200, 200) / 100;
            directionRight = true;
            ballColour.Color = Color.White;

            playerPoint.X = 0;
            playerPoint.Y = (panel1.Height - playerSize) / 2;
            playerDirection = ' ';
            playerSpeed = 6;

            drawMode = 'S';
            panel1.Refresh();
        }

        //flashing start message
        private void timer2_Tick(object sender, EventArgs e)
        {
            textFlash++;
            if (textFlash == 3)
            {
                textFlash = 0;
            }

            if (textFlash == 0)
            {
                drawMode = ' ';
                this.Refresh();
            }
            else
            {
                drawMode = 'S';
                this.Refresh();
            }
        }

        //draw screen, 'N' for normal draw mode, 'S' for start screen, any other entry for blank start screen
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle border1 = new Rectangle(0, 0, panel1.Width, borderWidth);
            Rectangle border2 = new Rectangle(0, panel1.Height - borderWidth, panel1.Width, borderWidth);
            Rectangle border3 = new Rectangle(panel1.Width - borderWidth, 0, borderWidth, panel1.Height);
            Rectangle[] borderArea = { border1, border2, border3 };
            Rectangle ballDraw = new Rectangle(0, 0, ballSize, ballSize);
            Rectangle playerBouncer = new Rectangle(0, 0, borderWidth, playerSize);
            SolidBrush colorWhite = new SolidBrush(Color.White);

            //draw boundaries
            e.Graphics.Clear(Color.Black);
            e.Graphics.FillRectangles(colorWhite, borderArea);
            //draw player
            playerBouncer.Location = playerPoint;
            e.Graphics.FillRectangle(colorWhite, playerBouncer);
            //draw ball and player score in colour
            if (drawMode == 'N' || drawMode == 'S')
            {
                ballDraw.Location = ballPoint;
                e.Graphics.FillEllipse(ballColour, ballDraw);
                using (Font scoreFont = new Font("Consolas", ballSize * 2, FontStyle.Bold))
                {
                    e.Graphics.DrawString(playerScore.ToString(), scoreFont, ballColour, borderWidth, borderWidth);
                }
            }
            //draw start message
            if (drawMode == 'S')
            {
                using (Font startFont = new Font("Consolas", ballSize, FontStyle.Bold))
                {
                    string beginMsg = "Press SPACE to Begin";
                    e.Graphics.DrawString(beginMsg, startFont, colorWhite, panel1.Width / 3, (panel1.Height - ballSize) / 2);
                }
                using (Font smallFont = new Font("Consolas", ballSize / 2, FontStyle.Bold))
                {
                    string instructionMsg = "Arrows or W/S to Move\r\nEsc to Exit";
                    e.Graphics.DrawString(instructionMsg, smallFont, colorWhite, borderWidth, panel1.Height - borderWidth - smallFont.Height * 2);
                }
            }
            colorWhite.Dispose();
        }
    }
}
