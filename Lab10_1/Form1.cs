﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Lab10_1
{
	public partial class Form1 : Form
	{
		private string c_name = "ABC Company", c_font = "Arial";
		private int c_size = 20;
		private Color c_color = Color.Black;
		private List<string> yrs = new List<string>();
		private List<string> rvn = new List<string>();
		private Color Lcolor = Color.Blue;
		private float[] Pattern = new float[] { 1 };
		private Point orgin;
		private int r = 255, g = 0, b = 0;
		private HatchStyle hatch = HatchStyle.BackwardDiagonal;

		public Form1()
		{
			InitializeComponent();
			sqlConnection1.Open();
			SqlDataReader reader = sqlCommand1.ExecuteReader();
			while(reader.Read())
			{
				yrs.Add(reader["Year"].ToString());
				rvn.Add(reader["Revenue"].ToString());
			}
			reader.Close();
			sqlConnection1.Close();
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			int y = 20;
			orgin = new Point(100, this.Height - 100);
			y += DrawStr(c_name, 0, y, c_size);
			y += DrawStr("Annual Revenue", 0, y, 18);
			int x = this.Width - 100;
			for(int i = yrs.Count - 1; i >= 0; i--, x -= 60)
			{
				DrawRect(yrs[i], x, y, 60, 60);
			}
			DrawRect("Year", x - 40, y, 100, 60);
			y += 60;
			x = this.Width - 100;
			for(int i = rvn.Count - 1; i >= 0; i--, x -= 60)
			{
				DrawRect(rvn[i], x, y, 60, 60);
			}
			DrawRect("Revenue", x - 40, y, 100, 60);
			y += 60;
			Pen pen = new Pen(Color.Black, 1);
			pen.EndCap = LineCap.ArrowAnchor;
			Pen penR = new Pen(Color.Red, 1);
			Font font = new Font("Arial", 16);
			SolidBrush brush = new SolidBrush(Color.Black);
			e.Graphics.DrawLine(pen, orgin.X, orgin.Y, orgin.X + 1100, orgin.Y);
			e.Graphics.DrawLine(pen, orgin.X, orgin.Y, orgin.X, orgin.Y - 940);
			pen.StartCap = LineCap.RoundAnchor;
			pen.EndCap = LineCap.RoundAnchor;
			for(int i = 0, cx = orgin.X; i < yrs.Count; i++, cx += 100)
			{
				e.Graphics.DrawString(yrs[i], font, brush, cx + 70, orgin.Y + 10);
				e.Graphics.DrawLine(penR, cx + 100, orgin.Y + 8, cx + 100, orgin.Y - ( int.Parse(rvn[i]) - 140 + 10 ) * 5);
			}
			for(int i = 140, cy = orgin.Y - 50; i <= 300; i += 10, cy -= 50)
			{
				e.Graphics.DrawString(i.ToString(), font, brush, orgin.X - 55, cy - 12);
				e.Graphics.DrawLine(pen, orgin.X - 8, cy, orgin.X + 1050, cy);
			}
			DrawGraph();
		}

		public int DrawStr( string str, int x, int y, int size = 18 )
		{
			Graphics graphic = this.CreateGraphics();
			Rectangle rect = new Rectangle(x, y, this.Width, size * 2);
			Font font = new Font(c_font, size);
			SolidBrush brush;
			if(size == 18)
				brush = new SolidBrush(Color.Black);
			else
				brush = new SolidBrush(c_color);
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			graphic.DrawString(str, font, brush, rect, sf);
			return size * 2;
		}

		private void companyNameToolStripMenuItem_Click( object sender, System.EventArgs e )
		{
			string temp_name = c_name, temp_font = c_font;
			int temp_size = c_size;
			Color temp_color = c_color;
			Form2 data = new Form2(this, c_name, c_font, c_size, c_color);
			DialogResult response = data.ShowDialog();
			c_name = data.Str;
			c_font = data.Fon;
			c_size = data.Siz;
			c_color = data.Colo;
			if(response == DialogResult.Cancel)
			{
				c_name = temp_name;
				c_font = temp_font;
				c_size = temp_size;
				c_color = temp_color;
			}
			Invalidate();
		}

		public void Apply( string c_name, string c_font, int c_size, Color c_color )
		{
			this.c_name = c_name;
			this.c_font = c_font;
			this.c_size = c_size;
			this.c_color = c_color;
			Invalidate();
		}

		public void DrawRect( string str, int x, int y, int w, int h )
		{
			Pen pen = new Pen(Color.Black, 3);
			Font font = new Font("Arial", 16);
			SolidBrush brush = new SolidBrush(Color.Black);
			Graphics graphic = this.CreateGraphics();
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			Rectangle rect = new Rectangle(x, y, w, h);
			graphic.DrawRectangle(pen, x, y, w, h);
			graphic.DrawString(str, font, brush, rect, sf);
		}

		public void DrawGraph()
		{
			Graphics graphic = this.CreateGraphics();
			Pen pen = new Pen(Lcolor, 3);
			pen.DashPattern = Pattern;
			HatchBrush brushR = new HatchBrush(hatch, Color.White, Color.FromArgb(175, r, g, b));
			int xx = orgin.X + 100;
			int yy = orgin.Y - ( int.Parse(rvn[0]) - 140 + 10 ) * 5;
			for(int i = 0, cx = orgin.X, cy = orgin.Y; i < rvn.Count && i < yrs.Count; i++, cy -= 50, cx += 100)
			{
				graphic.DrawLine(pen, xx, yy, cx + 100, orgin.Y - ( int.Parse(rvn[i]) - 140 + 10 ) * 5);
				xx = cx + 100;
				yy = orgin.Y - ( int.Parse(rvn[i]) - 140 + 10 ) * 5;
				Rectangle rect = new Rectangle(xx - 10, yy, 20, orgin.Y - yy);
				graphic.FillRectangle(brushR, rect);
			}
		}

		public void changeGraph()
		{
			Graphics graphic = this.CreateGraphics();
			Pen pen = new Pen(Lcolor, 3);
			pen.DashPattern = Pattern;
			int xx = orgin.X + 100;
			int yy = orgin.Y - ( int.Parse(rvn[0]) - 140 + 10 ) * 5;
			for(int i = 0, cx = orgin.X, cy = orgin.Y; i < rvn.Count && i < yrs.Count; i++, cy -= 50, cx += 100)
			{
				graphic.DrawLine(pen, xx, yy, cx + 100, orgin.Y - ( int.Parse(rvn[i]) - 140 + 10 ) * 5);
				xx = cx + 100;
				yy = orgin.Y - ( int.Parse(rvn[i]) - 140 + 10 ) * 5;
			}
		}

		private void Form1_KeyDown( object sender, KeyEventArgs e )
		{
			if(Control.ModifierKeys == Keys.Control)
			{
				switch(e.KeyCode)
				{
					case Keys.R:
					Lcolor = Color.Red;
					break;

					case Keys.G:
					Lcolor = Color.Green;
					break;

					case Keys.B:
					Lcolor = Color.Blue;
					break;
				}
				changeGraph();
			}
		}

		private void Form1_MouseUp( object sender, MouseEventArgs e )
		{
			int height = ( ( ( this.Height - 100 ) - e.Y ) / 5 ) + 130;
			if(e.Button == MouseButtons.Left && height <= 300 && height >= 140 && e.X >= 100 && e.X <= 1200)
			{
				MessageBox.Show(height.ToString());
			}
			if(e.Button == MouseButtons.Right && height <= 300 && height >= 140 && e.X >= 100 && e.X <= 1200)
			{
				//this.ContextMenuStrip = this.contextMenuStrip2;
				contextMenuStrip2.Show(PointToScreen(e.Location));
			}
			else
			{
				//this.ContextMenuStrip = this.contextMenuStrip1;
				contextMenuStrip1.Show(PointToScreen(e.Location));
			}
		}

		private void redToolStripMenuItem_Click( object sender, System.EventArgs e )
		{
			redToolStripMenuItem.Checked = true;
			greenToolStripMenuItem.Checked = false;
			blueToolStripMenuItem.Checked = false;
			redToolStripMenuItem2.Checked = true;
			greenToolStripMenuItem2.Checked = false;
			blueToolStripMenuItem2.Checked = false;
			Lcolor = Color.Red;
			changeGraph();
		}

		private void greenToolStripMenuItem_Click( object sender, System.EventArgs e )
		{
			redToolStripMenuItem.Checked = false;
			greenToolStripMenuItem.Checked = true;
			blueToolStripMenuItem.Checked = false;
			redToolStripMenuItem2.Checked = false;
			greenToolStripMenuItem2.Checked = true;
			blueToolStripMenuItem2.Checked = false;
			Lcolor = Color.Green;
			changeGraph();
		}

		private void blueToolStripMenuItem_Click( object sender, System.EventArgs e )
		{
			redToolStripMenuItem.Checked = false;
			greenToolStripMenuItem.Checked = false;
			blueToolStripMenuItem.Checked = true;
			redToolStripMenuItem2.Checked = false;
			greenToolStripMenuItem2.Checked = false;
			blueToolStripMenuItem2.Checked = true;
			Lcolor = Color.Blue;
			changeGraph();
		}

		private void solidToolStripMenuItem_Click( object sender, System.EventArgs e )
		{
			solidToolStripMenuItem.Checked = true;
			dashedToolStripMenuItem.Checked = false;
			dottedToolStripMenuItem.Checked = false;
			solidToolStripMenuItem2.Checked = true;
			dashedToolStripMenuItem2.Checked = false;
			dottedToolStripMenuItem2.Checked = false;
			Pattern = new float[] { 1 };
			Invalidate();
		}

		private void dashedToolStripMenuItem_Click( object sender, System.EventArgs e )
		{
			solidToolStripMenuItem.Checked = false;
			dashedToolStripMenuItem.Checked = true;
			dottedToolStripMenuItem.Checked = false;
			solidToolStripMenuItem2.Checked = false;
			dashedToolStripMenuItem2.Checked = true;
			dottedToolStripMenuItem2.Checked = false;
			Pattern = new float[] { 4, 1 };
			Invalidate();
		}

		private void dottedToolStripMenuItem_Click( object sender, System.EventArgs e )
		{
			solidToolStripMenuItem.Checked = false;
			dashedToolStripMenuItem.Checked = false;
			dottedToolStripMenuItem.Checked = true;
			solidToolStripMenuItem2.Checked = false;
			dashedToolStripMenuItem2.Checked = false;
			dottedToolStripMenuItem2.Checked = true;
			Pattern = new float[] { 2, 2 };
			Invalidate();
		}

		private void redToolStripMenuItem1_Click( object sender, System.EventArgs e )
		{
			redToolStripMenuItem1.Checked = true;
			greenToolStripMenuItem1.Checked = false;
			blueToolStripMenuItem1.Checked = false;
			redToolStripMenuItem3.Checked = true;
			greenToolStripMenuItem3.Checked = false;
			blueToolStripMenuItem3.Checked = false;
			r = 255;
			g = b = 0;
			Invalidate();
		}

		private void greenToolStripMenuItem1_Click( object sender, System.EventArgs e )
		{
			redToolStripMenuItem1.Checked = false;
			greenToolStripMenuItem1.Checked = true;
			blueToolStripMenuItem1.Checked = false;
			redToolStripMenuItem3.Checked = false;
			greenToolStripMenuItem3.Checked = true;
			blueToolStripMenuItem3.Checked = false;
			g = 255;
			r = b = 0;
			Invalidate();
		}

		private void blueToolStripMenuItem1_Click( object sender, System.EventArgs e )
		{
			redToolStripMenuItem1.Checked = false;
			greenToolStripMenuItem1.Checked = false;
			blueToolStripMenuItem1.Checked = true;
			redToolStripMenuItem3.Checked = false;
			greenToolStripMenuItem3.Checked = false;
			blueToolStripMenuItem3.Checked = true;
			b = 255;
			r = g = 0;
			Invalidate();
		}

		private void solidToolStripMenuItem1_Click( object sender, System.EventArgs e )
		{
			solidToolStripMenuItem1.Checked = true;
			dashedToolStripMenuItem1.Checked = false;
			dottedToolStripMenuItem1.Checked = false;
			rightToolStripMenuItem.Checked = true;
			leftToolStripMenuItem.Checked = false;
			crossToolStripMenuItem.Checked = false;
			hatch = HatchStyle.BackwardDiagonal;
			Invalidate();
		}

		private void dashedToolStripMenuItem1_Click( object sender, System.EventArgs e )
		{
			solidToolStripMenuItem1.Checked = false;
			dashedToolStripMenuItem1.Checked = true;
			dottedToolStripMenuItem1.Checked = false;
			rightToolStripMenuItem.Checked = false;
			leftToolStripMenuItem.Checked = true;
			crossToolStripMenuItem.Checked = false;
			hatch = HatchStyle.ForwardDiagonal;
			Invalidate();
		}

		private void dottedToolStripMenuItem1_Click( object sender, System.EventArgs e )
		{
			solidToolStripMenuItem1.Checked = false;
			dashedToolStripMenuItem1.Checked = false;
			dottedToolStripMenuItem1.Checked = true;
			rightToolStripMenuItem.Checked = false;
			leftToolStripMenuItem.Checked = false;
			crossToolStripMenuItem.Checked = true;
			hatch = HatchStyle.DiagonalCross;
			Invalidate();
		}
	}
}