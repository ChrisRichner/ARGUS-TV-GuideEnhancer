/*
 * Created by SharpDevelop.
 * User: geoff
 * Date: 04/10/2010
 * Time: 9:25 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ConfigTool
{
   partial class MainForm
   {
      /// <summary>
      /// Designer variable used to keep track of non-visual components.
      /// </summary>
      private System.ComponentModel.IContainer components = null;
      
      /// <summary>
      /// Disposes resources used by the form.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing) {
            if (components != null) {
               components.Dispose();
            }
         }
         base.Dispose(disposing);
      }
      
      /// <summary>
      /// This method is required for Windows Forms designer support.
      /// Do not change the method contents inside the source code editor. The Forms designer might
      /// not be able to load this method if it was changed manually.
      /// </summary>
      private void InitializeComponent()
      {
      	this.saveButton = new System.Windows.Forms.Button();
      	this.cancelButton = new System.Windows.Forms.Button();
      	this.SuspendLayout();
      	// 
      	// saveButton
      	// 
      	this.saveButton.Location = new System.Drawing.Point(336, 296);
      	this.saveButton.Name = "saveButton";
      	this.saveButton.Size = new System.Drawing.Size(75, 23);
      	this.saveButton.TabIndex = 0;
      	this.saveButton.Text = "Save";
      	this.saveButton.UseVisualStyleBackColor = true;
      	this.saveButton.Click += new System.EventHandler(this.Button1Click);
      	// 
      	// cancelButton
      	// 
      	this.cancelButton.Location = new System.Drawing.Point(413, 296);
      	this.cancelButton.Name = "cancelButton";
      	this.cancelButton.Size = new System.Drawing.Size(75, 23);
      	this.cancelButton.TabIndex = 0;
      	this.cancelButton.Text = "Cancel";
      	this.cancelButton.UseVisualStyleBackColor = true;
      	this.cancelButton.Click += new System.EventHandler(this.Button1Click);
      	// 
      	// MainForm
      	// 
      	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      	this.ClientSize = new System.Drawing.Size(500, 331);
      	this.Controls.Add(this.cancelButton);
      	this.Controls.Add(this.saveButton);
      	this.Name = "MainForm";
      	this.Text = "Guide Enricher Config Tool";
      	this.Load += new System.EventHandler(this.MainFormLoad);
      	this.ResumeLayout(false);
      }
      private System.Windows.Forms.Button saveButton;
      private System.Windows.Forms.Button cancelButton;
   }
}
