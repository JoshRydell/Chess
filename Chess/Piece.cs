using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
namespace Chess
{
    class Piece
    {
        public PictureBox Img { get; protected set; }
        private static int MoveCounter = 0;
        private static int NumPieces = 32;
        public const int squareSize = 60;
        public const int boardDimension = 8;
        protected static int WhiteKingX = 3;
        protected static int WhiteKingY = 0;
        protected static int BlackKingX = 3;
        protected static int BlackKingY = boardDimension - 1;
        protected static int InActivityCount = 0;
        private static bool WhiteMove = true;
        private static bool MoveMade = false;
        public static PictureBox BackBoard;
        public int X { get; protected set; }
        public int Y { get; protected set; }
        protected int endMoveX;
        protected int endMoveY;
        protected static Piece[,] Board = new Piece[boardDimension, boardDimension];
        public enum Colours : ushort
        {
            Black = 0,
            White = 1,
        };

        public PieceNames PieceName { get; protected set; }

        public enum PieceNames : ushort
        {
            King = 0,
            Queen = 1,
            Rook = 2,
            Knight = 3,
            Bishop = 4,
            Pawn = 5,
        };
        public Colours Colour { get; protected set; }
        private bool Move = false;
        private Point Start;
        protected static Bitmap LoadImage(int[] img)
        {
            Bitmap c = new Bitmap(60, 60);
            int counter = 0;
            for (int i = 0; i < 60; i++)
            {
                for (int j = 0; j < 60; j++)
                {
                    c.SetPixel(j, i, Color.FromArgb(img[counter]));
                    counter++;
                }
            }

            return c;
        }
        private void Select(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !MoveMade && (Colour == Colours.White && WhiteMove || Colour == Colours.Black && !WhiteMove))
            {
                Img.BringToFront();
                Move = true;
                Start = new Point(Cursor.Position.X, Cursor.Position.Y);

            }


        }
        private void Deselect(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Move = false;
                Point Centre = new Point(Img.Location.X + squareSize / 2, Img.Location.Y + squareSize / 2);
                Img.Location = new Point(Centre.X / squareSize * squareSize, Centre.Y / squareSize * squareSize);
                endMoveX = Img.Location.X / squareSize;
                endMoveY = Img.Location.Y / squareSize;
            }
        }
        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (Move)
            {
                Img.Location = new Point(Cursor.Position.X - Start.X + X * squareSize, Cursor.Position.Y - Start.Y + Y * squareSize);
            }

        }
        protected void Setup(Colours Colour, int X, int Y, Piece Piece)
        {
            this.X = X;
            this.Y = Y;
            Img = new PictureBox();
            Img.Size = new Size(squareSize, squareSize);
            Img.SizeMode = PictureBoxSizeMode.Zoom;
            Img.Location = new Point(X * squareSize, Y * squareSize);
            Img.BackColor = Color.Transparent;
            Img.MouseDown += new MouseEventHandler(Select);
            Img.MouseUp += new MouseEventHandler(Deselect);
            Img.MouseMove += new MouseEventHandler(MouseMove);
            this.Colour = Colour;
            Board[X, Y] = Piece;
            BackBoard.Controls.Add(Img);
            Img.BringToFront();
        }
        protected bool OffScreen()
        {
            return endMoveY < 0 || endMoveY >= boardDimension || endMoveX < 0 || endMoveX >= boardDimension;
        }
        protected bool TakeOwnPieceAttempt()
        {
            return Board[endMoveX, endMoveY] != null && Board[endMoveX, endMoveY].Colour == Colour;

        }
        protected void UpdateBoard()
        {
            InActivityCount++;
            MoveCounter++;
            TakePieceAttempt();
            Board[X, Y] = null;
            X = endMoveX;
            Y = endMoveY;
            Board[X, Y] = this;
            WhiteMove = !WhiteMove;
            if (Colour == Colours.White && CheckMate(Colours.Black))
            {
                if (Check(Colours.Black))
                {
                    MessageBox.Show("White Wins!");
                }
                else
                {
                    MessageBox.Show("Stalemate!");
                }

            }
            else if (Colour == Colours.Black && CheckMate(Colours.White))
            {
                if (Check(Colours.White))
                {
                    MessageBox.Show("Black Wins!");
                }
                else
                {
                    MessageBox.Show("Stalemate!");
                }

            }

            if (InActivityCount > 50)
            {
                MessageBox.Show("Draw due to inactivity");
            }
            //MoveMade = true;
        }
        private void TakePieceAttempt()
        {
            if (Board[endMoveX, endMoveY] != null && Board[endMoveX, endMoveY].Colour != Colour)
            {
                InActivityCount = 1;
                Board[endMoveX, endMoveY].Img.Dispose();
                NumPieces--;
            }
        }
        protected bool Check(Colours Colour)
        {
            int KingX = -1;
            int KingY = -1;

            Piece CurrentPiece = Board[endMoveX, endMoveY];
            Board[X, Y] = null;
            Board[endMoveX, endMoveY] = this;
            bool Outcome;

            if (Colour == Colours.White)
            {
                KingX = WhiteKingX;
                KingY = WhiteKingY;
            }
            else if (Colour == Colours.Black)
            {
                KingX = BlackKingX;
                KingY = BlackKingY;
            }

            Outcome = CheckForPawn(KingX, KingY, Colour) || CheckForRook(KingX, KingY, Colour) || CheckForKnight(KingX, KingY, Colour) || CheckForBishop(KingX, KingY, Colour) || CheckForQueen(KingX, KingY, Colour) || CheckForKing(KingX, KingY, Colour);
            Board[endMoveX, endMoveY] = CurrentPiece;
            Board[X, Y] = this;

            return Outcome;
        }
        private static bool Check(int endMoveX, int endMoveY, Piece Piece)
        {

            int KingX = -1;
            int KingY = -1;
            if (Piece.PieceName == PieceNames.King)
            {
                if (Piece.Colour == Colours.White)
                {
                    WhiteKingX = endMoveX;
                    WhiteKingY = endMoveY;
                }
                else if (Piece.Colour == Colours.Black)
                {
                    BlackKingX = endMoveX;
                    BlackKingY = endMoveY;
                }
            }

            Piece CurrentPiece = Board[endMoveX, endMoveY];
            Board[Piece.X, Piece.Y] = null;
            Board[endMoveX, endMoveY] = Piece;
            bool Outcome;

            if (Piece.Colour == Colours.White)
            {
                KingX = WhiteKingX;
                KingY = WhiteKingY;
            }
            else if (Piece.Colour == Colours.Black)
            {
                KingX = BlackKingX;
                KingY = BlackKingY;
            }

            Outcome = CheckForPawn(KingX, KingY, Piece.Colour) || CheckForRook(KingX, KingY, Piece.Colour) || CheckForKnight(KingX, KingY, Piece.Colour) || CheckForBishop(KingX, KingY, Piece.Colour) || CheckForQueen(KingX, KingY, Piece.Colour) || CheckForKing(KingX, KingY, Piece.Colour);
            Board[endMoveX, endMoveY] = CurrentPiece;
            Board[Piece.X, Piece.Y] = Piece;

            if (Piece.PieceName == PieceNames.King)
            {
                if (Piece.Colour == Colours.White)
                {
                    WhiteKingX = Piece.X;
                    WhiteKingY = Piece.Y;
                }
                else if (Piece.Colour == Colours.Black)
                {
                    BlackKingX = Piece.X;
                    BlackKingY = Piece.Y;
                }
            }

            return Outcome;
        }
        private static bool CheckForPiece(int X, int Y, PieceNames PieceType, Colours Colour)
        {
            if (X < 0 || Y < 0 || Y >= boardDimension || X >= boardDimension || Board[X, Y] == null)
            {
                return false;
            }

            return Board[X, Y].PieceName == PieceType && Board[X, Y].Colour != Colour;
        }
        private static bool CheckForPawn(int X, int Y, Colours Colour)
        {
            if (Colour == Colours.White)
            {
                return CheckForPiece(X - 1, Y + 1, PieceNames.Pawn, Colour) || CheckForPiece(X + 1, Y + 1, PieceNames.Pawn, Colour);
            }
            else if (Colour == Colours.Black)
            {
                return CheckForPiece(X - 1, Y - 1, PieceNames.Pawn, Colour) || CheckForPiece(X + 1, Y - 1, PieceNames.Pawn, Colour);
            }
            return true;
        }
        private static bool CheckForKing(int X, int Y, Colours Colour)
        {
            for (int i = X - 1; i <= X + 1; i++)
            {
                for (int j = Y - 1; j <= Y + 1; j++)
                {
                    if (CheckForPiece(i, j, PieceNames.King, Colour))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private static bool CheckForKnight(int X, int Y, Colours Colour)
        {
            return CheckForPiece(X - 1, Y - 2, PieceNames.Knight, Colour) || CheckForPiece(X + 1, Y - 2, PieceNames.Knight, Colour) || CheckForPiece(X - 1, Y + 2, PieceNames.Knight, Colour) || CheckForPiece(X + 1, Y + 2, PieceNames.Knight, Colour) || CheckForPiece(X - 2, Y - 1, PieceNames.Knight, Colour) || CheckForPiece(X + 2, Y - 1, PieceNames.Knight, Colour) || CheckForPiece(X - 2, Y + 1, PieceNames.Knight, Colour) || CheckForPiece(X + 2, Y + 1, PieceNames.Knight, Colour);
        }
        private static bool CheckForBishop(int X, int Y, Colours Colour)
        {
            bool NE = false;
            bool SE = false;
            bool SW = false;
            bool NW = false;


            for (int i = boardDimension; i > 0; i--)
            {
                if (X - i >= 0 && Y - i >= 0 && Board[X - i, Y - i] != null)
                {
                    NW = CheckForPiece(X - i, Y - i, PieceNames.Bishop, Colour);
                }

                if (X + i < boardDimension && Y + i < boardDimension && Board[X + i, Y + i] != null)
                {
                    SE = CheckForPiece(X + i, Y + i, PieceNames.Bishop, Colour);
                }

                if (X - i >= 0 && Y + i < boardDimension && Board[X - i, Y + i] != null)
                {
                    SW = CheckForPiece(X - i, Y + i, PieceNames.Bishop, Colour);
                }

                if (X + i < boardDimension && Y - i >= 0 && Board[X + i, Y - i] != null)
                {
                    NE = CheckForPiece(X + i, Y - i, PieceNames.Bishop, Colour);
                }
            }

            return NE || SE || SW || NW;


        }
        private static bool CheckForRook(int X, int Y, Colours Colour)
        {
            bool N = false;
            bool E = false;
            bool S = false;
            bool W = false;
            for (int i = boardDimension; i > 0; i--)
            {
                if (X - i >= 0 && Board[X - i, Y] != null)
                {
                    W = CheckForPiece(X - i, Y, PieceNames.Rook, Colour);
                }
                if (X + i < boardDimension && Board[X + i, Y] != null)
                {
                    E = CheckForPiece(X + i, Y, PieceNames.Rook, Colour);
                }
                if (Y - i >= 0 && Board[X, Y - i] != null)
                {
                    N = CheckForPiece(X, Y - i, PieceNames.Rook, Colour);
                }
                if (Y + i < boardDimension && Board[X, Y + i] != null)
                {
                    S = CheckForPiece(X, Y + i, PieceNames.Rook, Colour);
                }
            }

            return N || E || S || W;
        }
        private static bool CheckForQueen(int X, int Y, Colours Colour)
        {
            bool N = false;
            bool E = false;
            bool S = false;
            bool W = false;
            bool NE = false;
            bool SE = false;
            bool SW = false;
            bool NW = false;

            for (int i = boardDimension; i > 0; i--)
            {
                if (X - i >= 0 && Board[X - i, Y] != null)
                {
                    W = CheckForPiece(X - i, Y, PieceNames.Queen, Colour);
                }
                if (X + i < boardDimension && Board[X + i, Y] != null)
                {
                    E = CheckForPiece(X + i, Y, PieceNames.Queen, Colour);
                }
                if (Y - i >= 0 && Board[X, Y - i] != null)
                {
                    N = CheckForPiece(X, Y - i, PieceNames.Queen, Colour);
                }
                if (Y + i < boardDimension && Board[X, Y + i] != null)
                {
                    S = CheckForPiece(X, Y + i, PieceNames.Queen, Colour);
                }
                if (X - i >= 0 && Y - i >= 0 && Board[X - i, Y - i] != null)
                {
                    NW = CheckForPiece(X - i, Y - i, PieceNames.Queen, Colour);
                }

                if (X + i < boardDimension && Y + i < boardDimension && Board[X + i, Y + i] != null)
                {
                    SE = CheckForPiece(X + i, Y + i, PieceNames.Queen, Colour);
                }

                if (X - i >= 0 && Y + i < boardDimension && Board[X - i, Y + i] != null)
                {
                    SW = CheckForPiece(X - i, Y + i, PieceNames.Queen, Colour);
                }

                if (X + i < boardDimension && Y - i >= 0 && Board[X + i, Y - i] != null)
                {
                    NE = CheckForPiece(X + i, Y - i, PieceNames.Queen, Colour);
                }
            }

            return N || E || S || W || NE || SE || SW || NW;
        }
        private static bool CheckMate(Colours Colour)
        {
            for (int i = 0; i < boardDimension; i++)
            {
                for (int j = 0; j < boardDimension; j++)
                {
                    Piece CurrentPiece = Board[i, j];
                    if (CurrentPiece != null && CurrentPiece.Colour == Colour)
                    {
                        if (CurrentPiece.PieceName == PieceNames.Rook)
                        {
                            if (!CheckMateCheckQBR(1, 0, CurrentPiece) || !CheckMateCheckQBR(-1, 0, CurrentPiece) || !CheckMateCheckQBR(0, 1, CurrentPiece) || !CheckMateCheckQBR(0, -1, CurrentPiece))
                            {
                                return false;
                            }
                        }
                        else if (CurrentPiece.PieceName == PieceNames.Knight)
                        {
                            if (CheckMateCheckKnight(CurrentPiece))
                            {
                                return false;
                            }
                        }
                        else if (CurrentPiece.PieceName == PieceNames.Bishop)
                        {
                            if (!CheckMateCheckQBR(1, 1, CurrentPiece) || !CheckMateCheckQBR(-1, 1, CurrentPiece) || !CheckMateCheckQBR(1, -1, CurrentPiece) || !CheckMateCheckQBR(-1, -1, CurrentPiece))
                            {
                                return false;
                            }
                        }
                        else if (CurrentPiece.PieceName == PieceNames.Queen)
                        {
                            if (!CheckMateCheckQBR(1, 1, CurrentPiece) || !CheckMateCheckQBR(-1, 1, CurrentPiece) || !CheckMateCheckQBR(1, -1, CurrentPiece) || !CheckMateCheckQBR(-1, -1, CurrentPiece) || !CheckMateCheckQBR(1, 0, CurrentPiece) || !CheckMateCheckQBR(-1, 0, CurrentPiece) || !CheckMateCheckQBR(0, 1, CurrentPiece) || !CheckMateCheckQBR(0, -1, CurrentPiece))
                            {
                                return false;
                            }
                        }
                        else if (CurrentPiece.PieceName == PieceNames.Pawn)
                        {
                            if (!CheckMateCheckPawn((Pawn)CurrentPiece))
                            {
                                return false;
                            }
                        }
                        else if (CurrentPiece.PieceName == PieceNames.King)
                        {
                            if (!CheckMateCheckKing(CurrentPiece))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        private static bool CheckMateCheckQBR(int Xincrement, int Yincrement, Piece Piece)
        {
            int index = 1;
            while (Piece.Y + Yincrement * index < boardDimension && Piece.Y + Yincrement * index >= 0 && Piece.X + Xincrement * index < boardDimension && Piece.X + Xincrement * index >= 0 &&
                  (Board[Piece.X + Xincrement * index, Piece.Y + Yincrement * index] == null || Board[Piece.X + Xincrement * index, Piece.Y + Yincrement * index].Colour != Piece.Colour))
            {
                if (!Check(Piece.X + Xincrement * index, Piece.Y + Yincrement * index, Piece))
                {
                    return false;
                }

                if (Board[Piece.X + Xincrement * index, Piece.Y + Yincrement * index] != null && Board[Piece.X + Xincrement * index, Piece.Y + Yincrement * index].Colour != Piece.Colour)
                {
                    return true;
                }
                index++;
            }

            return true;
        }
        private static bool CheckMateCheckKnight(Piece Knight)
        {
            return Knight.X - 2 >= 0 && Knight.Y - 1 >= 0 && !Check(Knight.X - 2, Knight.Y - 1, Knight) ||
                Knight.X - 2 >= 0 && Knight.Y + 1 < boardDimension && !Check(Knight.X - 2, Knight.Y + 1, Knight) ||
                Knight.X + 2 < boardDimension && Knight.Y - 1 >= 0 && !Check(Knight.X + 2, Knight.Y - 1, Knight) ||
                Knight.X + 2 < boardDimension && Knight.Y + 1 < boardDimension && !Check(Knight.X + 2, Knight.Y + 1, Knight) ||
                Knight.X - 1 >= 0 && Knight.Y - 2 >= 0 && !Check(Knight.X - 1, Knight.Y - 2, Knight) ||
                Knight.X - 1 >= 0 && Knight.Y + 2 < boardDimension && !Check(Knight.X - 1, Knight.Y + 2, Knight) ||
                Knight.X + 1 < boardDimension && Knight.Y - 2 >= 0 && !Check(Knight.X + 1, Knight.Y - 2, Knight) ||
                Knight.X + 1 < boardDimension && Knight.Y + 2 < boardDimension && !Check(Knight.X + 1, Knight.Y + 2, Knight);
        }
        private static bool CheckMateCheckPawn(Pawn Pawn)
        {
            if (Pawn.Colour == Colours.Black && (Board[Pawn.X, Pawn.Y - 1] == null && !Check(Pawn.X, Pawn.Y - 1, Pawn) || Pawn.FirstMove && Board[Pawn.X, Pawn.Y - 2] == null && !Check(Pawn.X, Pawn.Y - 2, Pawn) || Pawn.X - 1 >= 0 && Board[Pawn.X - 1, Pawn.Y - 1] != null && !Check(Pawn.X - 1, Pawn.Y - 1, Pawn) || Pawn.X + 1 < boardDimension && Board[Pawn.X + 1, Pawn.Y - 1] != null && !Check(Pawn.X + 1, Pawn.Y - 1, Pawn)))
            {
                return false;
            }
            else if (Pawn.Colour == Colours.White && (Board[Pawn.X, Pawn.Y + 1] == null && !Check(Pawn.X, Pawn.Y + 1, Pawn) || Pawn.FirstMove && Board[Pawn.X, Pawn.Y + 2] == null && !Check(Pawn.X, Pawn.Y + 2, Pawn) || Pawn.X - 1 >= 0 && Board[Pawn.X - 1, Pawn.Y + 1] != null && !Check(Pawn.X - 1, Pawn.Y + 1, Pawn) || Pawn.X + 1 < boardDimension && Board[Pawn.X + 1, Pawn.Y + 1] != null && !Check(Pawn.X + 1, Pawn.Y + 1, Pawn)))
            {
                return false;
            }
            return true;
        }
        private static bool CheckMateCheckKing(Piece King)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (King.X + i >= 0 && King.X + i < boardDimension && King.Y + j >= 0 && King.Y + j < boardDimension && Board[King.X + i, King.Y + j] == null && !Check(King.X + i, King.Y + j, King))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        public static void SaveBoardState(object sender, MouseEventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.FileName = "Move " + MoveCounter.ToString();
            fd.Filter = "Chess Board State (*.cbs)|*.cbs";
            fd.Title = "Save Board State";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                BinaryWriter writer = new BinaryWriter(new FileStream(fd.FileName, FileMode.Create));
                writer.Write(WhiteMove);
                writer.Write(MoveCounter);
                writer.Write((ushort)NumPieces);
                
                for (int i = 0; i < boardDimension; i++)
                {
                    for (int j = 0; j < boardDimension; j++)
                    {
                        if (Board[i, j] != null)
                        {
                            if(Board[i,j].PieceName == PieceNames.Rook)
                            {
                                writer.Write((ushort)0);
                            }
                            else if (Board[i, j].PieceName == PieceNames.Knight)
                            {
                                writer.Write((ushort)1);
                            }
                            else if (Board[i, j].PieceName == PieceNames.Bishop)
                            {
                                writer.Write((ushort)2);
                            }
                            else if (Board[i, j].PieceName == PieceNames.King)
                            {
                                writer.Write((ushort)3);
                            }
                            else if (Board[i, j].PieceName == PieceNames.Queen)
                            {
                                writer.Write((ushort)4);
                            }
                            else if (Board[i, j].PieceName == PieceNames.Pawn)
                            {
                                writer.Write((ushort)5);
                            }

                            writer.Write(Board[i, j].Colour == Colours.White);
                            writer.Write((ushort)Board[i, j].X);
                            writer.Write((ushort)Board[i, j].Y);

                        }
                    } 
                }

                writer.Close();
            }
        }
        public static void LoadBoardState(object sender, MouseEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Chess Board State (*.cbs)|*.cbs";
            fd.Title = "Load Board State";
            if (fd.ShowDialog() == DialogResult.OK)
            {

                for(int i = 0; i < boardDimension; i++)
                {
                    for(int j = 0; j < boardDimension; j++)
                    {
                        if(Board[i,j] != null)
                        {
                            Board[i, j].Img.Hide();
                            Board[i, j] = null;
                        }
                    }
                }

                MoveMade = false;
                BinaryReader reader = new BinaryReader(new FileStream(fd.FileName, FileMode.Open));
                WhiteMove = reader.ReadBoolean();
                MoveCounter = reader.ReadInt32();
                NumPieces = reader.ReadUInt16();

                for (int i = 0; i < NumPieces; i++)
                {
                    int PieceType = reader.ReadUInt16();
                    bool White = reader.ReadBoolean();
                    int X = reader.ReadUInt16();
                    int Y = reader.ReadUInt16();
                    if(PieceType == 0)
                    {
                        Board[X, Y] = new Rook(White ? Colours.White : Colours.Black, X, Y);
                    }
                    else if (PieceType == 1)
                    {
                        Board[X, Y] = new Knight(White ? Colours.White : Colours.Black, X, Y);
                    }
                    else if (PieceType == 2)
                    {
                        Board[X, Y] = new Bishop(White ? Colours.White : Colours.Black, X, Y);
                    }
                    else if (PieceType == 3)
                    {
                        Board[X, Y] = new King(White ? Colours.White : Colours.Black, X, Y);
                        if(White)
                        {
                            WhiteKingX = X;
                            WhiteKingY = Y;
                        }
                        else
                        {
                            BlackKingX = X;
                            BlackKingY = Y;
                        }
                    }
                    else if (PieceType == 4)
                    {
                        Board[X, Y] = new Queen(White ? Colours.White : Colours.Black, X, Y);
                    }
                    else if (PieceType == 5)
                    {
                        Board[X, Y] = new Pawn(White ? Colours.White : Colours.Black, X, Y);
                    }

                }

                reader.Close();
            }


        }
        
    }
}
