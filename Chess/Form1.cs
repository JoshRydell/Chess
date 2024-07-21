using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Chess
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Text = "Chess";
            ClientSize = new Size(Piece.squareSize * Piece.boardDimension, Piece.squareSize * Piece.boardDimension + 75);
            MaximumSize = Size;
            //BackColor = Color.Blue;

            PictureBox BoardImage = new PictureBox();
            BoardImage.Size = Size;
            BoardImage.Location = new Point(0, 0);
            Controls.Add(BoardImage);
            Piece.BackBoard = BoardImage;

            Bitmap b = new Bitmap(Piece.squareSize * Piece.boardDimension, Piece.squareSize * Piece.boardDimension);
            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.FromArgb(50,50,50));

            for(int i = 0; i < Piece.boardDimension /2; i++)
            {
                for(int j = 0; j < Piece.boardDimension /2; j++)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(230,230,230)), (i + 0.5f) * 2 * Piece.squareSize, (j + 0.5f) * 2 * Piece.squareSize, Piece.squareSize, Piece.squareSize);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(230, 230, 230)), i * 2 * Piece.squareSize, j * 2 * Piece.squareSize, Piece.squareSize, Piece.squareSize);
                }
            }
            BoardImage.Image = b;

            Piece[] Pieces = new Piece[32]
            {
                new Rook(Piece.Colours.White, 0, 0),
                new Knight(Piece.Colours.White, 1, 0),
                new Bishop(Piece.Colours.White, 2, 0),
                new King(Piece.Colours.White, 3, 0),
                new Queen(Piece.Colours.White, 4, 0),
                new Bishop(Piece.Colours.White, 5, 0),
                new Knight(Piece.Colours.White, 6, 0),
                new Rook(Piece.Colours.White, 7, 0),
                new Pawn(Piece.Colours.White, 0, 1),
                new Pawn(Piece.Colours.White, 1, 1),
                new Pawn(Piece.Colours.White, 2, 1),
                new Pawn(Piece.Colours.White, 3, 1),
                new Pawn(Piece.Colours.White, 4, 1),
                new Pawn(Piece.Colours.White, 5, 1),
                new Pawn(Piece.Colours.White, 6, 1),
                new Pawn(Piece.Colours.White, 7, 1),
                new Rook(Piece.Colours.Black, 0, Piece.boardDimension - 1),
                new Knight(Piece.Colours.Black, 1, Piece.boardDimension - 1),
                new Bishop(Piece.Colours.Black, 2, Piece.boardDimension - 1),
                new King(Piece.Colours.Black, 3, Piece.boardDimension - 1),
                new Queen(Piece.Colours.Black, 4, Piece.boardDimension - 1),
                new Bishop(Piece.Colours.Black, 5, Piece.boardDimension - 1),
                new Knight(Piece.Colours.Black, 6, Piece.boardDimension - 1),
                new Rook(Piece.Colours.Black, 7, Piece.boardDimension - 1),
                new Pawn(Piece.Colours.Black, 0, Piece.boardDimension - 2),
                new Pawn(Piece.Colours.Black, 1, Piece.boardDimension - 2),
                new Pawn(Piece.Colours.Black, 2, Piece.boardDimension - 2),
                new Pawn(Piece.Colours.Black, 3, Piece.boardDimension - 2),
                new Pawn(Piece.Colours.Black, 4, Piece.boardDimension - 2),
                new Pawn(Piece.Colours.Black, 5, Piece.boardDimension - 2),
                new Pawn(Piece.Colours.Black, 6, Piece.boardDimension - 2),
                new Pawn(Piece.Colours.Black, 7, Piece.boardDimension - 2),
            };


            Button Save = new Button();
            Save.Size = new Size(75, 50);
            Save.Location = new Point(ClientSize.Width / 2 - Save.Width - 10, Piece.squareSize * Piece.boardDimension + 15);
            Save.MouseClick += new MouseEventHandler(Piece.SaveBoardState);
            Save.Text = "Save Board State";
            Controls.Add(Save);
            Save.BringToFront();

            Button Load = new Button();
            Load.Size = Save.Size;
            Load.Location = new Point(Save.Location.X + Save.Width + 20, Piece.squareSize * Piece.boardDimension + 15);
            Load.MouseClick += new MouseEventHandler(Piece.LoadBoardState);
            
            Load.Text = "Load Board State";
            Controls.Add(Load);
            Load.BringToFront();
        }
    }
}

    
    

