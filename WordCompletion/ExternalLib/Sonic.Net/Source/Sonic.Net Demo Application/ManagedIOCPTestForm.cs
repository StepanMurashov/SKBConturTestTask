using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

using Sonic.Net;

namespace Sonic.Net.Demo
{
	public delegate void Disp(Label lbl,string str);
	public delegate void EnableStartDelegate(ManagedIOCPTestForm frm);
	/// <summary>
	/// Summary description for ManagedIOCPTestForm.
	/// </summary>
	public class ManagedIOCPTestForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button StartCmd;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label ATh;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Button PauseContinueCmd;
		private System.Windows.Forms.TextBox txtObjCount;

		#region Managed IOCP related static data

		private static AutoResetEvent _ev = new AutoResetEvent(false);
		private static ManagedIOCP _mIOCP = new ManagedIOCP(3);
		private static Label _lbl = null;
		private static int _objCount = 0;
		private static long _st = 0;
		private static long _et = 0;
		private static bool _bDone = false;
		private System.Windows.Forms.Button StopCmd;
		private static int _staticCount = 0;

		#endregion

		public ManagedIOCPTestForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.StartCmd = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.ATh = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.txtObjCount = new System.Windows.Forms.TextBox();
			this.PauseContinueCmd = new System.Windows.Forms.Button();
			this.StopCmd = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(136, 48);
			this.label1.Name = "label1";
			this.label1.TabIndex = 0;
			this.label1.Text = "Th1";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(136, 80);
			this.label2.Name = "label2";
			this.label2.TabIndex = 1;
			this.label2.Text = "Th2";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(136, 112);
			this.label3.Name = "label3";
			this.label3.TabIndex = 2;
			this.label3.Text = "Th3";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(136, 144);
			this.label4.Name = "label4";
			this.label4.TabIndex = 3;
			this.label4.Text = "Th4";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(136, 176);
			this.label5.Name = "label5";
			this.label5.TabIndex = 4;
			this.label5.Text = "Th4";
			// 
			// StartCmd
			// 
			this.StartCmd.Location = new System.Drawing.Point(416, 224);
			this.StartCmd.Name = "StartCmd";
			this.StartCmd.TabIndex = 2;
			this.StartCmd.Text = "&Start";
			this.StartCmd.Click += new System.EventHandler(this.StartCmd_Click);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// ATh
			// 
			this.ATh.Location = new System.Drawing.Point(288, 48);
			this.ATh.Name = "ATh";
			this.ATh.Size = new System.Drawing.Size(176, 144);
			this.ATh.TabIndex = 6;
			this.ATh.Text = "label6";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(32, 48);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(80, 23);
			this.label6.TabIndex = 7;
			this.label6.Text = "Thread 1:";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(32, 80);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(80, 23);
			this.label7.TabIndex = 8;
			this.label7.Text = "Thread 2:";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(32, 112);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(80, 23);
			this.label8.TabIndex = 9;
			this.label8.Text = "Thread 3:";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(32, 144);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(80, 23);
			this.label9.TabIndex = 10;
			this.label9.Text = "Thread 4:";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(32, 176);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(80, 23);
			this.label10.TabIndex = 11;
			this.label10.Text = "Thread 5:";
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(16, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(232, 192);
			this.groupBox1.TabIndex = 12;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Objects processed by threads";
			// 
			// groupBox2
			// 
			this.groupBox2.Location = new System.Drawing.Point(256, 16);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(232, 192);
			this.groupBox2.TabIndex = 13;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "ManagedIOCP object statistics";
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(168, 224);
			this.numericUpDown1.Maximum = new System.Decimal(new int[] {
																		   5,
																		   0,
																		   0,
																		   0});
			this.numericUpDown1.Minimum = new System.Decimal(new int[] {
																		   1,
																		   0,
																		   0,
																		   0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(40, 20);
			this.numericUpDown1.TabIndex = 0;
			this.numericUpDown1.Value = new System.Decimal(new int[] {
																		 3,
																		 0,
																		 0,
																		 0});
			this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(16, 227);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(120, 16);
			this.label11.TabIndex = 15;
			this.label11.Text = "Concurrent Threads";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(16, 256);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(120, 16);
			this.label12.TabIndex = 16;
			this.label12.Text = "Number of objects";
			// 
			// txtObjCount
			// 
			this.txtObjCount.Location = new System.Drawing.Point(168, 256);
			this.txtObjCount.Name = "txtObjCount";
			this.txtObjCount.TabIndex = 1;
			this.txtObjCount.Text = "10000";
			// 
			// PauseContinueCmd
			// 
			this.PauseContinueCmd.Location = new System.Drawing.Point(416, 248);
			this.PauseContinueCmd.Name = "PauseContinueCmd";
			this.PauseContinueCmd.TabIndex = 4;
			this.PauseContinueCmd.Text = "&Pause";
			this.PauseContinueCmd.Click += new System.EventHandler(this.PauseContinueCmd_Click);
			// 
			// StopCmd
			// 
			this.StopCmd.Location = new System.Drawing.Point(416, 224);
			this.StopCmd.Name = "StopCmd";
			this.StopCmd.TabIndex = 3;
			this.StopCmd.Text = "S&top";
			this.StopCmd.Visible = false;
			this.StopCmd.Click += new System.EventHandler(this.StopCmd_Click);
			// 
			// ManagedIOCPTestForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(504, 325);
			this.Controls.Add(this.StopCmd);
			this.Controls.Add(this.PauseContinueCmd);
			this.Controls.Add(this.txtObjCount);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.ATh);
			this.Controls.Add(this.StartCmd);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox2);
			this.Name = "ManagedIOCPTestForm";
			this.Text = "ManagedIOCP demo application";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new ManagedIOCPTestForm());
		}

		private void StartCmd_Click(object sender, System.EventArgs e)
		{
            _mIOCP = new ManagedIOCP(Convert.ToInt32(numericUpDown1.Value));
			StartCmd.Visible = false;
			StopCmd.Visible = true;

			_staticCount = 1;
			_bDone = false;
			label1.Text = "0";
			label2.Text = "0";
			label3.Text = "0";
			label4.Text = "0";
			label5.Text = "0";
			ATh.Text = "Registered Threads	: 0\n\n" +
					   "Active Threads		: 0\n\n" +
					   "Queue Count			: 0\n\n" +
					   "Running Status		: True\n\n" + 
						"Elapsed Time (ms)	: 0";	
			_objCount = Convert.ToInt32(txtObjCount.Text);
			_mIOCP.ConcurrentThreads = Convert.ToInt32(numericUpDown1.Value);

			for(int i=1;i<=5;i++)
			{
				if (i==1) 
					_lbl = label1;
				else if (i==2) 
					_lbl = label2;
				else if (i==3) 
					_lbl = label3;
				else if (i==4) 
					_lbl = label4;
				else if (i==5) 
					_lbl = label5;
				
				Thread th = new Thread(new ThreadStart(ThreadRun));
				th.Start();

				_ev.WaitOne();
			}
			
			// Start our timer
			//
			_st = DateTime.Now.Ticks;

			// Trigger the dispatch loop. Starts multiple producer and
			// multiple consumer pattern
			//
			_mIOCP.Dispatch(_staticCount);
			
			/*
			 * Single Producer pattern
			for(int j = 1; j <= _objCount; j++)
			{
				_mIOCP.Dispatch(j);
			}
			*/
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			string stats = string.Format(
						"Registered Threads: {0}\n\n" +
						"Active Threads: {1}\n\n" +
						"Queue Count: {2}\n\n" +
						"Running Status: {3}\n\n" + 
						"Elapsed Time (ms): {4}",
						_mIOCP.RegisteredThreads,
						_mIOCP.ActiveThreads,
						_mIOCP.QueuedObjectCount,
						_mIOCP.IsRunning.ToString(),
						(_et - _st)/10000);

			ATh.Text = stats;
			ATh.Refresh();
		}

		internal void EnableStart()
		{
			StopCmd.Visible = false;
			StartCmd.Visible = true;
		}

		#region Managed IOCP related static methods

		private static void EnableStartStatic(ManagedIOCPTestForm frm)
		{
			frm.EnableStart();	
		}
		private static void Display(Label lbl, string str)
		{
			lbl.Text = str;
		}
		private static void ThreadRun()
		{
			IOCPHandle hIOCP = _mIOCP.Register();
			Label lbl = _lbl;
			_ev.Set();
			int j = 1;
			while(!_bDone)
			{
				try
				{
					object obj = hIOCP.Wait();
					if (obj != null)
					{
						int i = (int)obj;
						if (i >= _objCount) 
						{
							_et = DateTime.Now.Ticks;
							_bDone = true;
							MessageBox.Show("Done");
							_mIOCP.Close();
							_lbl.Parent.Invoke(new EnableStartDelegate(EnableStartStatic),new object[] { (ManagedIOCPTestForm)_lbl.Parent });
						}
						else
						{
							_mIOCP.Dispatch(Interlocked.Increment(ref _staticCount));
						}

						// Since this is not the thread where label object (lbl) is 
						// created, we cannot directly change its text. This restriction
						// is true for any window object. 
						// So we update the label object (lbl) text by calling invoke
						// on lbl object and passing our delegate. Invoke internally
						// calls the delegate on the thread that created lbl object
						//
						lbl.Invoke(new Disp(Display),new object[] {lbl,j.ToString()});
						j++;
					}
				}
				catch(Exception e)
				{
					if ((e is ManagedIOCPException) == false)
						MessageBox.Show(e.Message + " --- " + e.StackTrace);
					_bDone = true;
				}
			}
			MessageBox.Show("Unregistering Thread " + AppDomain.GetCurrentThreadId());
			_mIOCP.UnRegister();
		}

		#endregion

		private void PauseContinueCmd_Click(object sender, System.EventArgs e)
		{
			if (PauseContinueCmd.Text == "Pause")
			{
				_mIOCP.Pause();
				PauseContinueCmd.Text = "Continue";
			}
			else
			{
				_mIOCP.Run();
				PauseContinueCmd.Text = "Pause";
			}
		}

		private void numericUpDown1_ValueChanged(object sender, System.EventArgs e)
		{
			_mIOCP.ConcurrentThreads = Convert.ToInt32(numericUpDown1.Value);
		}

		private void StopCmd_Click(object sender, System.EventArgs e)
		{
			_et = DateTime.Now.Ticks;
			_mIOCP.Close();
			StopCmd.Visible = false;
			StartCmd.Visible = true;

			// Our Managed IOCP has already come into Run mode.
			// So just bring back our PauseContinue button to Pause
			// state
			PauseContinueCmd.Text = "Pause";
		}
	}
}
