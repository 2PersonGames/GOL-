using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace GOL.Objects
{
    class GameOfLife
    {
        //GOL[column, row]
        //false = dead
        //true = alive
        bool[,] currentGOL;
        public bool[,] CurrentGOL
        {
            get { return currentGOL; }
            set
            {
                for (int i = 0; i < currentGOL.Length; i++)
                {
                    for (int j = 0; j < currentGOL.Length; j++)
                    {
                        currentGOL[i, j] = value[i, j] ;
                    }
                }
            }
        }
        bool[,] previousGOL;
        public bool[,] PreviousGOL
        {
            get { return previousGOL; }
            set
            {
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        previousGOL[i, j] = value[i, j];
                    }
                }
            }
        }
        
        List<Point> deadPositions;

        Rectangle[,] gridRect;

        Texture2D controllerA;
        Texture2D controllerB;
        Texture2D controllerX;

        Texture2D rules;
        public Texture2D Rules
        {
            get { return rules; }
            set { rules = value; }
        }
        Rectangle rulesRect;
        public Rectangle RulesRect
        {
            get { return rulesRect; }
        }
        Texture2D gameOverSprite;
        bool gameOver;
        public bool GameOver
        {
            get { return gameOver; }
            set { gameOver = value; }
        }
        
        int[,] neighbours;
        public int[,] Neighbours
        {
            get { return neighbours; }
            set
            {
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        neighbours[i, j] = value[i, j];
                    }
                }
            }
        }

        int height;
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                if (height < 1)
                {
                    height = 1;
                }
            }
        }
        int width;
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                if (width < 1)
                {
                    width = 1;
                }
            }
        }

        Rectangle drawAreaRect;
        public Rectangle DrawAreaRect
        {
            get { return drawAreaRect; }
        }
        Rectangle drawSquareRect;
        Texture2D grid;
        public Texture2D Grid
        {
            get { return grid; }
            set { grid = value; }
        }
        Texture2D square;
        public Texture2D Square
        {
            get { return square; }
            set { square = value; }
        }

        public GameOfLife(Texture2D nSquare, Texture2D nGrid, Texture2D b, Texture2D a, Texture2D x, Texture2D nGameOver, Texture2D nRules, int nMapWidth, int nMapHeight, Rectangle viewportRect)
        {
            //Instance code
            Square = nSquare;
            Grid = nGrid;
            gameOver = false;
            gameOverSprite = nGameOver;
            rules = nRules;

            Height = nMapHeight;
            Width = nMapWidth;

            currentGOL = new bool[Width, Height];
            previousGOL = new bool[Width, Height];
            neighbours = new int[Width, Height];

            drawAreaRect = new Rectangle(viewportRect.X + viewportRect.Width - viewportRect.Height, viewportRect.Y, viewportRect.Height, viewportRect.Height);
            drawSquareRect = new Rectangle(drawAreaRect.X, drawAreaRect.Y, drawAreaRect.Width / Width, drawAreaRect.Height / Height);
            rulesRect = new Rectangle(viewportRect.X, viewportRect.Y, viewportRect.Width - drawAreaRect.Width, viewportRect.Height);
            rulesRect.Height = rulesRect.Width;

            controllerB = b;
            controllerA = a;
            controllerX = x;
        }

        public void Load(int gOLVersion)
        {
            //Code for different setups
            switch (gOLVersion)
            {
                case 0:
                    {
                        //Custom version
                        gridRect = new Rectangle[Width, Height];
                        for (int i = 0; i < Width; i++)
                        {
                            for (int j = 0; j < Height; j++)
                            {
                                CurrentGOL[i, j] = false;
                                gridRect[i, j].Y = (drawSquareRect.Height * j) + drawAreaRect.Y;
                                gridRect[i, j].X = (drawSquareRect.Width * i) + drawAreaRect.X;
                                gridRect[i, j].Width = drawSquareRect.Width;
                                gridRect[i, j].Height = drawSquareRect.Height;
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        //Glider gun
                        GliderGun();
                        break;
                    }
                case 2:
                    {
                        BlockLayingSwitchEngineOne();
                        break;
                    }
                case 3:
                    {
                        BlockLayingSwitchEngineTwo();
                        break;
                    }
                case 4:
                    {
                        BlockLayingSwitchEngineThree();
                        break;
                    }
                case 5:
                    {
                        BiBlock();
                        break;
                    }
                case 99:
                    {
                        for (int i = 0; i < Width; i++)
                        {
                            for (int j = 0; j < Height; j++)
                            {
                                CurrentGOL[i, j] = false;
                                neighbours[i, j] = 0;
                            }
                        }
                        break;
                    }
            }
        }

        public void Update()
        {           
            if (!gameOver)
            {
                //Update code
                //First thing, make a copy of the current game state
                PreviousGOL = CurrentGOL;

                //Apply rules
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        CalculateNeighbours(i, j);
                    }
                }

                CheckIfStillLife();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Drawing code
            if (gameOver)
            {
                spriteBatch.Draw(gameOverSprite, new Rectangle(rulesRect.X + rulesRect.Width, rulesRect.Y, rulesRect.Width, rulesRect.Height), Color.DarkBlue);
            }

            Color drawColour;
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    drawColour = GetNeighbourColour(neighbours[i, j]);
                    drawSquareRect.Y = (drawSquareRect.Height * j) + drawAreaRect.Y;
                    drawSquareRect.X = (drawSquareRect.Width * i) + drawAreaRect.X;
                    if (currentGOL[i, j])
                    {
                        spriteBatch.Draw(Square, drawSquareRect, drawColour);
                    }
                    else if (!gameOver)
                    {
                        spriteBatch.Draw(Square, drawSquareRect, Color.Black);
                    }
                    spriteBatch.Draw(Grid, drawSquareRect, Color.Gray);
                }
            }
        }

        void CalculateNeighbours(int row, int column)
        {
            int aliveNeighbours = 0;

            if (row > 0 && previousGOL[row - 1, column])
            {
                aliveNeighbours++;
            }
            if (column > 0 && row > 0 && previousGOL[row - 1, column - 1])
            {
                aliveNeighbours++;
            }
            if (column > 0 && previousGOL[row, column - 1])
            {
                aliveNeighbours++;
            }
            if (column > 0 && row < Width - 1 && previousGOL[row + 1, column - 1])
            {
                aliveNeighbours++;
            }
            if (column < Height - 1 && row < Width - 1 && previousGOL[row + 1, column])
            {
                aliveNeighbours++;
            }
            if (column < Height - 1 && row < Width - 1 && previousGOL[row + 1, column + 1])
            {
                aliveNeighbours++;
            }
            if (column < Height - 1 && previousGOL[row, column + 1])
            {
                aliveNeighbours++;
            }
            if (column < Height - 1 && row > 0 && previousGOL[row - 1, column + 1])
            {
                aliveNeighbours++;
            }

            neighbours[row, column] = aliveNeighbours;
            ApplyRules(row, column);
        }

        void ApplyRules(int row, int column)
        {
            if (previousGOL[row, column])
            {
                if (neighbours[row, column] < 2)
                {
                    //Dies of loneliness
                    currentGOL[row, column] = false;
                }
                else if (neighbours[row, column] > 3)
                {
                    //Dies of overcrowding
                    currentGOL[row, column] = false;
                }
            }
            else if (neighbours[row, column] == 3)
            {
                //Resurrect
                currentGOL[row, column] = true;
            }
        }

        Color GetNeighbourColour(int aliveNeighbours)
        {
            //Returns a colour depending on the number of alive neighbours
            switch (aliveNeighbours)
            {
                default:
                    {
                        return Color.White;
                    }
                case 1:
                    {
                        return Color.Violet;
                    }
                case 2:
                    {
                        return Color.DarkViolet;
                    }
                case 3:
                    {
                        return Color.Purple;
                    }
            }
        }

        //Create gliders on the 15th generation,
        //Then every 30 generations after
        void GliderGun()
        {
            //Needs width >= 36 and height >= 9
            if (Width >= 36 && Height >= 9)
            {
                CurrentGOL[0, 4] = true;
                CurrentGOL[0, 5] = true;
                CurrentGOL[1, 4] = true;
                CurrentGOL[1, 5] = true;

                CurrentGOL[10, 4] = true;
                CurrentGOL[10, 5] = true;
                CurrentGOL[10, 6] = true;
                CurrentGOL[11, 3] = true;
                CurrentGOL[11, 7] = true;
                CurrentGOL[12, 2] = true;
                CurrentGOL[12, 8] = true;
                CurrentGOL[13, 2] = true;
                CurrentGOL[13, 8] = true;
                CurrentGOL[14, 5] = true;
                CurrentGOL[15, 3] = true;
                CurrentGOL[15, 7] = true;
                CurrentGOL[16, 4] = true;
                CurrentGOL[16, 5] = true;
                CurrentGOL[16, 6] = true;
                CurrentGOL[17, 5] = true;

                CurrentGOL[20, 2] = true;
                CurrentGOL[20, 3] = true;
                CurrentGOL[20, 4] = true;
                CurrentGOL[21, 2] = true;
                CurrentGOL[21, 3] = true;
                CurrentGOL[21, 4] = true;
                CurrentGOL[22, 1] = true;
                CurrentGOL[22, 5] = true;
                CurrentGOL[24, 0] = true;
                CurrentGOL[24, 1] = true;
                CurrentGOL[24, 5] = true;
                CurrentGOL[24, 6] = true;

                CurrentGOL[34, 2] = true;
                CurrentGOL[34, 3] = true;
                CurrentGOL[35, 2] = true;
                CurrentGOL[35, 3] = true;
            }
        }

        //Next three create patterns which lay blocks indefinately
        void BlockLayingSwitchEngineOne()
        {
            //Needs width >= 10 and height >= 8
            if (Width >= 10 && Height >= 8)
            {
                CurrentGOL[1, 6] = true;

                CurrentGOL[3, 5] = true;
                CurrentGOL[3, 6] = true;

                CurrentGOL[5, 2] = true;
                CurrentGOL[5, 3] = true;
                CurrentGOL[5, 4] = true;

                CurrentGOL[7, 1] = true;
                CurrentGOL[7, 2] = true;
                CurrentGOL[7, 3] = true;

                CurrentGOL[8, 2] = true;
            }
        }

        void BlockLayingSwitchEngineTwo()
        {
            //Needs width >= 7 and height >= 7
            if (Width >= 7 && Height >= 7)
            {
                CurrentGOL[1, 1] = true;
                CurrentGOL[1, 2] = true;
                CurrentGOL[1, 5] = true;

                CurrentGOL[2, 1] = true;
                CurrentGOL[2, 4] = true;

                CurrentGOL[3, 1] = true;
                CurrentGOL[3, 4] = true;
                CurrentGOL[3, 5] = true;

                CurrentGOL[4, 3] = true;

                CurrentGOL[5, 1] = true;
                CurrentGOL[5, 3] = true;
                CurrentGOL[5, 4] = true;
                CurrentGOL[5, 5] = true;
            }
        }

        void BlockLayingSwitchEngineThree()
        {
            //Needs width >= 41 and height >= 3
            if (Width >= 7 && Height >= 7)
            {
                CurrentGOL[1, 1] = true;
                CurrentGOL[2, 1] = true;
                CurrentGOL[3, 1] = true;
                CurrentGOL[4, 1] = true;
                CurrentGOL[5, 1] = true;
                CurrentGOL[6, 1] = true;
                CurrentGOL[7, 1] = true;
                CurrentGOL[8, 1] = true;

                CurrentGOL[10, 1] = true;
                CurrentGOL[11, 1] = true;
                CurrentGOL[12, 1] = true;
                CurrentGOL[13, 1] = true;
                CurrentGOL[14, 1] = true;

                CurrentGOL[18, 1] = true;
                CurrentGOL[19, 1] = true;
                CurrentGOL[20, 1] = true;

                CurrentGOL[27, 1] = true;
                CurrentGOL[28, 1] = true;
                CurrentGOL[29, 1] = true;
                CurrentGOL[30, 1] = true;
                CurrentGOL[31, 1] = true;
                CurrentGOL[32, 1] = true;
                CurrentGOL[33, 1] = true;

                CurrentGOL[35, 1] = true;
                CurrentGOL[36, 1] = true;
                CurrentGOL[37, 1] = true;
                CurrentGOL[38, 1] = true;
                CurrentGOL[39, 1] = true;
            }
        }

        void BiBlock()
        {
            //Needs width >= 41 and height >= 3
            if (Width >= 25 && Height >= 12)
            {
                CurrentGOL[7, 19] = true;
                CurrentGOL[7, 20] = true;

                CurrentGOL[8, 17] = true;
                CurrentGOL[8, 18] = true;
                CurrentGOL[8, 19] = true;
                CurrentGOL[8, 21] = true;

                CurrentGOL[9, 16] = true;
                CurrentGOL[9, 22] = true;

                CurrentGOL[10, 16] = true;
                CurrentGOL[10, 17] = true;
                CurrentGOL[10, 23] = true;

                CurrentGOL[11, 19] = true;
                CurrentGOL[11, 20] = true;
                CurrentGOL[11, 24] = true;

                CurrentGOL[12, 16] = true;
                CurrentGOL[12, 17] = true;
                CurrentGOL[12, 19] = true;
                CurrentGOL[12, 20] = true;
                CurrentGOL[12, 23] = true;
                CurrentGOL[12, 24] = true;

                CurrentGOL[13, 16] = true;
                CurrentGOL[13, 17] = true;
                CurrentGOL[13, 23] = true;

                CurrentGOL[14, 19] = true;
                CurrentGOL[14, 20] = true;
                CurrentGOL[14, 24] = true;

                CurrentGOL[15, 20] = true;
                CurrentGOL[15, 23] = true;
                CurrentGOL[15, 24] = true;

                CurrentGOL[16, 20] = true;
                CurrentGOL[16, 22] = true;

                CurrentGOL[17, 21] = true;
                CurrentGOL[17, 22] = true;

                CurrentGOL[21, 15] = true;
                CurrentGOL[21, 16] = true;

                CurrentGOL[22, 15] = true;
                CurrentGOL[22, 18] = true;
                CurrentGOL[22, 19] = true;
                CurrentGOL[22, 21] = true;
                CurrentGOL[22, 22] = true;
                CurrentGOL[22, 24] = true;

                CurrentGOL[23, 16] = true;
                CurrentGOL[23, 18] = true;
                CurrentGOL[23, 20] = true;
                CurrentGOL[23, 21] = true;
                CurrentGOL[23, 23] = true;
                CurrentGOL[23, 24] = true;

                CurrentGOL[24, 15] = true;
                CurrentGOL[24, 16] = true;

                CurrentGOL[25, 14] = true;
                CurrentGOL[25, 18] = true;
                CurrentGOL[25, 20] = true;
                CurrentGOL[25, 21] = true;
                CurrentGOL[25, 23] = true;
                CurrentGOL[25, 24] = true;

                CurrentGOL[26, 15] = true;
                CurrentGOL[26, 16] = true;
                CurrentGOL[26, 18] = true;
                CurrentGOL[26, 19] = true;
                CurrentGOL[26, 21] = true;
                CurrentGOL[26, 23] = true;

                CurrentGOL[27, 16] = true;
                CurrentGOL[27, 24] = true;

                CurrentGOL[28, 16] = true;
                CurrentGOL[28, 18] = true;
                CurrentGOL[28, 19] = true;
                CurrentGOL[28, 21] = true;
                CurrentGOL[28, 23] = true;
                CurrentGOL[28, 24] = true;

                CurrentGOL[29, 17] = true;
                CurrentGOL[29, 18] = true;
                CurrentGOL[29, 20] = true;
                CurrentGOL[29, 21] = true;
                CurrentGOL[29, 23] = true;

                CurrentGOL[30, 25] = true;

                CurrentGOL[31, 24] = true;
                CurrentGOL[31, 25] = true;
            }
        }

        public void CustomUpdate(Point pointer)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (gridRect[i, j].Contains(pointer))
                    {
                        if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed)
                        {
                            CurrentGOL[i, j] = false;
                        }
                        if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed)
                        {
                            CurrentGOL[i, j] = true;
                        }
                    }
                }
            }
        }

        void CheckIfStillLife()
        {
            bool endGame = true;
            deadPositions = new List<Point>();

            CheckForBlinkers(currentGOL, previousGOL);
            CheckForToads(currentGOL, previousGOL);
            CheckForBeacons(currentGOL, previousGOL); 
            CheckForBlinkers(previousGOL, currentGOL);
            CheckForToads(previousGOL, currentGOL);
            CheckForBeacons(previousGOL, currentGOL);

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    bool endLoop = false;
                    foreach (Point deadPoint in deadPositions)
                    {
                        if (i == deadPoint.X && j == deadPoint.Y)
                        {
                            endLoop = true;
                            break;
                        }
                    }
                    if (!endLoop)
                    {
                        if (currentGOL[i, j] != previousGOL[i, j])
                        {
                            endGame = false;
                        }
                    }
                }
            }

            if (endGame)
            {
                gameOver = true;
            }
        }

        void CheckForBlinkers(bool[,] gol1, bool[,] gol2)
        {
            //Blinker
            for (int i = 0; i < Width - 2; i++)
            {
                for (int j = 0; j < Height - 2; j++)
                {
                    if (!gol1[i, j] && !gol1[i + 1, j] && !gol1[i + 2, j] &&
                        gol1[i, j + 1] && gol1[i + 1, j + 1] && gol1[i + 2, j + 1] &&
                        !gol1[i, j + 2] && !gol1[i + 1, j + 2] && !gol1[i + 2, j + 2] &&
                        !gol2[i, j + 1] && !gol2[i, j + 2] &&
                        gol2[i + 1, j] && gol2[i + 1, j + 1] && gol2[i + 1, j + 2] &&
                        !gol2[i + 2, j + 2] && !gol2[i + 2, j + 2] && !gol2[i + 2, j + 2])
                    {
                        for (int p = 0; p < 3; p++)
                        {
                            for (int q = 0; q < 3; q++)
                            {
                                deadPositions.Add(new Point(p + i, q + j));
                            }
                        }
                    }
                }
            }
        }

        void CheckForToads(bool[,] gol1, bool[,] gol2)
        {
            //Check for toads
            for (int i = 0; i < Width - 3; i++)
            {
                for (int j = 0; j < Height - 3; j++)
                {
                    if (!gol1[i, j] && gol1[i, j + 1] && gol1[i, j + 2] && !gol1[i, j + 3] &&
                        !gol1[i + 1, j] && !gol1[i + 1, j + 1] && !gol1[i + 1, j + 2] && gol1[i + 1, j + 3] &&
                        gol1[i + 2, j] && !gol1[i + 2, j + 1] && !gol1[i + 2, j + 2] && !gol1[i + 2, j + 3] &&
                        !gol1[i + 3, j] && gol1[i + 3, j + 1] && gol1[i + 3, j + 2] && !gol1[i + 3, j + 3] &&
                        !gol2[i, j] && !gol2[i, j + 1] && gol2[i, j + 2] && !gol2[i, j + 3] &&
                        !gol2[i + 1, j] && gol2[i + 1, j + 1] && gol2[i + 1, j + 2] && !gol2[i + 1, j + 3] &&
                        !gol2[i + 2, j] && gol2[i + 2, j + 1] && gol2[i + 2, j + 2] && !gol2[i + 2, j + 3] &&
                        !gol2[i + 3, j] && gol2[i + 3, j + 1] && !gol2[i + 3, j + 2] && !gol2[i + 3, j + 3])
                    {
                        for (int p = 0; p < 4; p++)
                        {
                            for (int q = 0; q < 4; q++)
                            {
                                deadPositions.Add(new Point(p + i, q + j));
                            }
                        }
                    }
                }
            }
        }

        void CheckForBeacons(bool[,] gol1, bool[,] gol2)
        {
            //Check for beacons
            for (int i = 0; i < Width - 3; i++)
            {
                for (int j = 0; j < Height - 3; j++)
                {
                    if (gol1[i, j] && gol1[i, j + 1] && !gol1[i, j + 2] && !gol1[i, j + 3] &&
                        gol1[i + 1, j] && !gol1[i + 1, j + 1] && !gol1[i + 1, j + 2] && !gol1[i + 1, j + 3] &&
                        !gol1[i + 2, j] && !gol1[i + 2, j + 1] && !gol1[i + 2, j + 2] && gol1[i + 2, j + 3] &&
                        !gol1[i + 3, j] && !gol1[i + 3, j + 1] && gol1[i + 3, j + 2] && gol1[i + 3, j + 3] &&
                        gol2[i, j] && gol2[i, j + 1] && !gol2[i, j + 2] && !gol2[i, j + 3] &&
                        gol2[i + 1, j] && gol2[i + 1, j + 1] && !gol2[i + 1, j + 2] && !gol2[i + 1, j + 3] &&
                        !gol2[i + 2, j] && !gol2[i + 2, j + 1] && gol2[i + 2, j + 2] && gol2[i + 2, j + 3] &&
                        !gol2[i + 3, j] && !gol2[i + 3, j + 1] && gol2[i + 3, j + 2] && gol2[i + 3, j + 3])
                    {
                        for (int p = 0; p < 4; p++)
                        {
                            for (int q = 0; q < 4; q++)
                            {
                                deadPositions.Add(new Point(p + i, q + j));
                            }
                        }
                    }
                }
            }
        }
    }
}
