using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CTAApp
{
    public partial class Form1 : Form
    {
        string connectionInfo;
        string remoteInfo;
        public Form1()
        {
            InitializeComponent();
            //
            // TextApp
            //
            // Sharad Tanwar
            // U. of Illinois, Chicago
            // CS480, Summer 2015
            // Homework 5 - App
            //
            // 
            // constructor: setup connection info
            //
            string filename = "CTA_DB.mdf";

            connectionInfo
                = String.Format(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\{0};Integrated Security=True;",
              filename);
            listBox2.Items.Add(@"Please use the application as follows:
                
               
            ");
            listBox2.Items.Add(" 1. Give FilePath for both file as indicated in the textboxes");
            listBox2.Items.Add(" 2. Purge Database if data already present. Takes 5-10 seconds");
            listBox2.Items.Add(" 3. Load Data by clicking the button load data.Takes 5-10 seconds");
            listBox2.Items.Add(" 4. Click on Show details Once data is finished loading.");
            textBox1.Text = String.Format(@"C:\Users\Sharad\Desktop\CTA-L-Ridership-Daily.csv");
            textBox5.Text = String.Format(@"C:\Users\Sharad\Desktop\CTA-L-Stops-2.csv");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection db;
            bool done = false;
            string checksql,sql, sql2, msg;
            SqlCommand cmd;
            object result, result2;
            SqlTransaction tx = null;
            //db = new SqlConnection(connectionInfo);
            try
            {
                db = new SqlConnection(connectionInfo);
                db.Open();
            }
            catch(Exception Ex){
                string msg2 = string.Format("Open Failed,'{0}'", Ex.Message);
                MessageBox.Show(msg2);
                return;
            }

            try
            {
                tx = db.BeginTransaction(IsolationLevel.Serializable); // No duplicates
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Cant begin Transaction due to : " + Ex);
                db.Close();
                return;
            }
            // Another try catch block checking on the database. DELETE Records if already present.

            listBox2.Items.Clear();
            try
            {
                try
                {
                    // C:\Users\Sharad\Desktop\CTA-L-Ridership-Daily.csv
                    string ridership_file_path = textBox1.Text;
                    string ridership_error_file = textBox1.Text.Replace(".csv","_error.txt");
                    sql2 = string.Format(@"
                                        DECLARE @bulk_cmd VARCHAR(1000)
                                        SET @bulk_cmd = '
                                            BULK INSERT dbo.Ridership_Intermediate
                                            FROM ''{0}''
                                            WITH
                                            (
							                FIRSTROW =2,
                                            FIELDTERMINATOR ='','',
                                            ROWTERMINATOR ='''+CHAR(10)+''',
                                            ERRORFILE = ''{1}''
                                            )'

							                EXEC(@bulk_cmd);

                            ", ridership_file_path, ridership_error_file);

                    //MessageBox.Show(sql2);
                    //MessageBox.Show(sql);
                    cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql2;
                    cmd.Transaction = tx;
                    int rowsModified = cmd.ExecuteNonQuery();

                    //MessageBox.Show("Ridership count" + rowsModified);
                    //textBox1.Text = textBox1.Text + "Total RiderShip Records Count :" + rowsModified + "\n";
                    listBox2.Items.Add("Total RiderShip Records Count :" + rowsModified);
                 
                }
                catch (Exception Ex)
                {
                    tx.Rollback();
                    //MessageBox.Show("Something bad happened" + Ex);
                    return;
                }


                try
                {
                    // C:\Users\Sharad\Desktop\CTA-L-Stops-2.csv
                    string stop_file_path = textBox5.Text;
                    string stop_error_file_path = textBox5.Text.Replace(".csv","_error.txt");
                    sql = string.Format(@"
                                        DECLARE @bulk_cmd VARCHAR(1000)
                                        SET @bulk_cmd = '
                                            BULK INSERT dbo.Stops_Intermediate
                                          FROM ''{0}''
                                          WITH
                                          (
									      FIRSTROW =2,
                                          FIELDTERMINATOR ='','',
                                          ROWTERMINATOR ='''+CHAR(10)+''',
                                           ERRORFILE = ''{1}''
                                          )'

									      EXEC(@bulk_cmd);

                            ",stop_file_path,stop_error_file_path);

                    //MessageBox.Show(sql2);
                    //MessageBox.Show(sql);
                    cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql;
                    cmd.Transaction = tx;
                    int rowsModified = cmd.ExecuteNonQuery();

                    //MessageBox.Show("Stops Count" + rowsModified);
                    //textBox1.Text = textBox1.Text + "\n" + "Number of Stop Records inserted :" + rowsModified;
                    listBox2.Items.Add("Number of Stop Records inserted :" + rowsModified);

                    try
                    {
                        sql = string.Format(@"
                                Select Station_Name, 
                                        count(S1.Station_Name)
			                        FROM Stops_Intermediate S1
			                    GROUP BY S1.Station_Name 
			                    HAVING COUNT(S1.Station_Name) = 1;");

                        cmd.Connection = db;
                        cmd.CommandText = sql;
                        cmd.Transaction = tx;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        DataSet ds = new DataSet();
                        //
                        adapter.Fill(ds);
                        //
                        DataTable dt = ds.Tables["TABLE"];
                        //

                        string text = "";

                        foreach (DataRow row in dt.Rows)
                        {

                            msg = string.Format("Station Name : {0} ,",
                                row["Station_Name"].ToString()
                            );

                            //MessageBox.Show(msg);
                            text = text + msg;
                        }
                        // textBox1.Text = textBox1.Text + "\n" + text ;
                        listBox2.Items.Add("Stations having mre than one names: " + text);
       

                    }
                    catch (Exception staEx)
                    {
                        tx.Rollback();
                        MessageBox.Show("Error While checking for a station with different names" + staEx.Message);
                    }
                    try
                    {
                        sql = string.Format(@"
                                    INSERT 
                                    INTO Station_Info(station_id,station_name,Station_Descriptive_Name)  
                                    SELECT 
                                    DISTINCT 
	                                    Map_ID,
	                                    (SELECT TOP 1 Station_Name FROM Stops_Intermediate S
			                                    WHERE S.Map_ID = S1.Map_ID ) Station_Name,
	                                    (SELECT TOP 1 Station_Descriptive_Name FROM Stops_Intermediate S
			                                    WHERE S.Map_ID = S1.Map_ID ) Station_Descriptive_Name
	                                    FROM Stops_Intermediate S1
	                                    ORDER BY Map_ID ASC;");

                        cmd = new SqlCommand();
                        cmd.Connection = db;
                        cmd.CommandText = sql;
                        cmd.Transaction = tx;
                        int rowsModified1 = cmd.ExecuteNonQuery();

                        //MessageBox.Show("Total Number of Stations : " + rowsModified1);
                        //textBox1.Text = textBox1.Text + "\n" +"Total Number of Stations : " + rowsModified1;
                        listBox2.Items.Add("Total Number of Stations : " + rowsModified1);
                 
                    }
                    catch(Exception Exx){
                        tx.Rollback();
                        MessageBox.Show("Exception Occured : {0}", Exx.Message);
                        return;
                    }

                    try
                    {
                        sql = string.Format(@"INSERT INTO Stops(Stop_id,
				                              station_id, 
			                                  Stop_Name,
				                              Direction,
				                              Handicap_Accessible,
				                              Location)
                            SELECT DISTINCT 
	                               Stop_ID,
	                               Map_id,
	                               Stop_Name,
	                               (CASE  
	                               WHEN Direction_ID = 'E'
		                            THEN 'East'
	                               WHEN Direction_ID = 'W'
		                            THEN 'West'
		                            WHEN Direction_ID = 'S'
		                            THEN 'South'
		                            WHEN Direction_ID = 'N'
		                            Then 'North'
		                            Else 'N/A'
		                            END ) Direction,
	                               ADA,
	                               Location 
                              FROM Stops_Intermediate
                              ;");

                            cmd = new SqlCommand();
                            cmd.Connection = db;
                            cmd.CommandText = sql;
                            cmd.Transaction = tx;
                            int rowsModified2 = cmd.ExecuteNonQuery();
                            //MessageBox.Show("Total Number of Stops : " + rowsModified2);
                            //textBox1.Text = textBox1.Text + "\n" + "Total Number of Stops : " + rowsModified2;
                            listBox2.Items.Add("Total Number of Stops : " + rowsModified2);
                    }
                    catch(Exception ex2){
                        tx.Rollback();
                        MessageBox.Show("Something happend while inserting stops {0}",ex2.Message);
                        return;
                    }
                    try
                    {
                        sql = string.Format(@"
                                INSERT INTO Lines_Info(Line_Name) VALUES('Pink');
                                INSERT INTO Lines_Info(Line_Name) VALUES('Orange');
                                INSERT INTO Lines_Info(Line_Name) VALUES('Red');
                                INSERT INTO Lines_Info(Line_Name) VALUES('Blue');
                                INSERT INTO Lines_Info(Line_Name) VALUES('Brown');
                                INSERT INTO Lines_Info(Line_Name) VALUES('Green');
                                INSERT INTO Lines_Info(Line_Name) VALUES('Purple');
                                INSERT INTO Lines_Info(Line_Name) VALUES('Purple-Express');
                                INSERT INTO Lines_Info(Line_Name) VALUES('Yellow');
                        ");
                        cmd = new SqlCommand();
                        cmd.Connection = db;
                        cmd.CommandText = sql;
                        cmd.Transaction = tx;
                        int rowsModified3 = cmd.ExecuteNonQuery();
                        //MessageBox.Show("Total Number of Stations : " + rowsModified3);
                        //textBox1.Text = textBox1.Text + "\n" + "INSERTING LINES : " + rowsModified3;
                        listBox2.Items.Add("Total Number of CTA Lines  : " + rowsModified3);
                        
                    }
                    catch(Exception ex3){
                        tx.Rollback();
                        MessageBox.Show("Exception Occured while inserting lines {0}", ex3.Message);
                        return;
                    }

                    try
                    {
                        sql = string.Format(@"
                                 INSERT INTO Stop_Details(Stop_id,Line_id)
                                  SELECT Stop_id, 
		                                (SELECT Line_id
		                                  FROM Lines_Info
		                                 WHERE Line_Name = 'Pink'
		                                )  Line_id
                                    FROM Stops_Intermediate
                                   WHERE UPPER(Pnk) = UPPER('True');

                                     INSERT INTO Stop_Details(Stop_id,Line_id)
                                  SELECT Stop_id, 
		                                (SELECT Line_id
		                                  FROM Lines_Info
		                                 WHERE Line_Name = 'Red'
		                                )  Line_id
                                    FROM Stops_Intermediate
                                   WHERE UPPER(RED) = UPPER('True');

                                        INSERT INTO Stop_Details(Stop_id,Line_id)
                                  SELECT Stop_id, 
		                                (SELECT Line_id
		                                  FROM Lines_Info
		                                 WHERE Line_Name = 'Blue'
		                                )  Line_id
                                    FROM Stops_Intermediate
                                   WHERE UPPER(BLUE) = UPPER('True');

                                           INSERT INTO Stop_Details(Stop_id,Line_id)
                                  SELECT Stop_id, 
		                                (SELECT Line_id
		                                  FROM Lines_Info
		                                 WHERE Line_Name = 'Green'
		                                )  Line_id
                                    FROM Stops_Intermediate
                                   WHERE UPPER(G) = UPPER('True');

                                              INSERT INTO Stop_Details(Stop_id,Line_id)
                                  SELECT Stop_id, 
		                                (SELECT Line_id
		                                  FROM Lines_Info
		                                 WHERE Line_Name = 'Brown'
		                                )  Line_id
                                    FROM Stops_Intermediate
                                   WHERE UPPER(BRN) = UPPER('True');

                                 INSERT INTO Stop_Details(Stop_id,Line_id)
                                  SELECT Stop_id, 
		                                (SELECT Line_id
		                                  FROM Lines_Info
		                                 WHERE Line_Name = 'Purple'
		                                )  Line_id
                                    FROM Stops_Intermediate
                                   WHERE UPPER(P) = UPPER('True');

                                                 INSERT INTO Stop_Details(Stop_id,Line_id)
                                  SELECT Stop_id, 
		                                (SELECT Line_id
		                                  FROM Lines_Info
		                                 WHERE Line_Name = 'Purple-Express'
		                                )  Line_id
                                    FROM Stops_Intermediate
                                   WHERE UPPER(Pexp) = UPPER('True');

                                 INSERT INTO Stop_Details(Stop_id,Line_id)
                                  SELECT Stop_id, 
		                                (SELECT Line_id
		                                  FROM Lines_Info
		                                 WHERE Line_Name = 'Yellow'
		                                )  Line_id
                                    FROM Stops_Intermediate
                                   WHERE UPPER(Y) = UPPER('True');

                                 INSERT INTO Stop_Details(Stop_id,Line_id)
                                  SELECT Stop_id, 
		                                (SELECT Line_id
		                                  FROM Lines_Info
		                                 WHERE Line_Name = 'Orange'
		                                )  Line_id
                                    FROM Stops_Intermediate
                                   WHERE UPPER(O) = UPPER('True');");

                        cmd = new SqlCommand();
                        cmd.Connection = db;
                        cmd.CommandText = sql;
                        cmd.Transaction = tx;
                        int rowsModified4 = cmd.ExecuteNonQuery();

                        //MessageBox.Show("Populating Stop Details : " + rowsModified4);
                        //textBox1.Text = textBox1.Text + "\n" +"Populating Stops and CTA LINES : " + rowsModified4;
                        listBox2.Items.Add("Populating Stops and CTA LINES : " + rowsModified4);
                    }
                    catch (Exception ex4)
                    {
                        tx.Rollback();
                        MessageBox.Show("Exception Occrured while inserting Stop details{0}",ex4.Message);
                        return;
                    }

                    // Checking Foreign Key Constraint while inserting data into Ridership Table.
                    try
                    {

                        try {
                            sql = string.Format(@"
                                    SELECT distinct station_id,
                                                    stationname 
                                    FROM Ridership_Intermediate
		                            WHERE station_id 
                                                NOT IN (SELECT DISTINCT 
									                            Station_Id from Station_Info 
									                            );
                            ");

                            cmd.Connection = db;
                            cmd.CommandText = sql;
                            cmd.Transaction = tx;

                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                            DataSet ds = new DataSet();
                            //
                            adapter.Fill(ds);
                            //
                            DataTable dt = ds.Tables["TABLE"];
                            //
                            
                            string text="";

                            foreach (DataRow row in dt.Rows)
                            {

                                msg = string.Format("Station Id : {0}, StationName : {1}",
                                    row["station_id"].ToString(),
                                    row["stationname"].ToString()
                                );

                                //MessageBox.Show(msg);
                                text = text + msg ;
                            }
                           // textBox1.Text = textBox1.Text + "\n" + text ;
                            listBox2.Items.Add("Stations with ridership data but without station names and id : ");
                            listBox2.Items.Add(text);
                        }
                        catch (Exception Fex)
                        {
                            tx.Rollback();
                            MessageBox.Show("Error Occured while selecting data {0}", Fex.Message);
                            return;
                        }
                        try{
                           sql = string.Format(@"
                            INSERT INTO Riderships(station_id, ride_date, daytype,rides)
                                SELECT station_id, date, daytype,rides
                                    FROM 
                                    Ridership_Intermediate
		                            WHERE station_id IN (SELECT DISTINCT 
									                            Station_Id from Station_Info 
									                        );
                            ");
                        cmd = new SqlCommand();
                        cmd.Connection = db;
                        cmd.CommandText = sql;
                        cmd.Transaction = tx;
                        int rowsModified3 = cmd.ExecuteNonQuery();
                        //MessageBox.Show("Total Number of Stations : " + rowsModified3);
                        //textBox1.Text = textBox1.Text + "\n" + "INSERTING LINES : " + rowsModified3;
                        listBox2.Items.Add("Populating final Ridership Table" + rowsModified3);
                        }
                        catch(Exception REX){
                            tx.Rollback();
                            //textBox1.Text = textBox1.Text + "\n" + "Error While inserting ";
                            MessageBox.Show("Error Occured while inserting due to {0}",REX.Message);
                            return;
                        }

                    }
                    catch (Exception ex3)
                    {
                        tx.Rollback();
                        MessageBox.Show("Exception Occured while inserting lines {0}", ex3.Message);
                        return;
                    }
                }
                catch (Exception Ex)
                {
                    tx.Rollback();
                    MessageBox.Show("Something bad happened while inserting into Stops {0}",Ex.Message);
                    return;
                }

                tx.Commit();
            }
            catch(Exception Ex)
            {
                tx.Rollback();
                MessageBox.Show("Could not insert records in the database. Please Check Error File");
                return;
            } 
            finally
            {
                db.Close();
            }

    
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection db;
            bool done = false;
            string checksql, sql, sql2, msg;
            SqlCommand cmd;
            object result, result2;
            db = new SqlConnection(connectionInfo);
            db.Open();
            SqlTransaction tx = null;
            listBox2.Items.Clear();
            try
            {
                tx = db.BeginTransaction(IsolationLevel.Serializable); // No duplicates
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Cant begin Transaction {0}",Ex.Message);
                db.Close();
                return;
            }

            try
            {

                try
                {
                    sql = string.Format(@"
                            DELETE FROM Ridership_Intermediate;
                    ");
                    cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql;
                    cmd.Transaction = tx;
                    int rowsModified = cmd.ExecuteNonQuery();

                   // MessageBox.Show("Records Deleted From Intermediate Table : " + rowsModified);

                    //textBox1.Text = textBox1.Text + "\r\n"+"Records Deleted From Intermediate Table : " + rowsModified;
                    listBox2.Items.Add("Records Deleted From Intermediate Table : " + rowsModified);
                }
                catch (Exception REX)
                {
                    tx.Rollback();
                    //textBox1.Text = textBox1.Text + "\r\n" + "Ridership Intermediate Could not be deleted. " ;
                    MessageBox.Show("Ridership Intermediate Could not be deleted. {0}", REX.Message);
                    return;
                }

                try {
                    sql = string.Format(@"DELETE FROM Stops_Intermediate;");
                    cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql;
                    cmd.Transaction = tx;
                    int rowsModified = cmd.ExecuteNonQuery();

                    //MessageBox.Show("Records Deleted From Intermediate Table : " + rowsModified);
                    //textBox1.Text = textBox1.Text + "\r\n"+ "Records Deleted From Intermediate Table : " + rowsModified;
                    listBox2.Items.Add("Records Deleted From Intermediate Table : " + rowsModified);
                
                }
                catch (Exception StoEX)
                {
                    tx.Rollback();
                    //textBox1.Text = textBox1.Text + "\r\n" + "Stops Intermediate Could not be deleted. ";
                    MessageBox.Show("Stops Intermediate Could not be deleted.", StoEX.Message);
                    return;
                }
                try {
                    sql = string.Format(@"DELETE FROM Riderships;");
                    cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql;
                    cmd.Transaction = tx;
                    int rowsModified = cmd.ExecuteNonQuery();

                    //MessageBox.Show("Records Deleted From Ridership Table : " + rowsModified);
                   // textBox1.Text = textBox1.Text + "\r\n" +  "Records Deleted From Ridership Table : " + rowsModified;
                    listBox2.Items.Add("Records Deleted From Ridership Table : " + rowsModified);
                 
                }
                catch (Exception RiEx)
                {
                    tx.Rollback();
                    //textBox1.Text = textBox1.Text +"\r\n" + "Riderships Could not be deleted. ";
                    MessageBox.Show("Riderships Could not be deleted. {0}",RiEx.Message);
                    return;
                }


                try {
                    sql = string.Format(@"DELETE FROM Stop_Details;");
                    cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql;
                    cmd.Transaction = tx;
                    int rowsModified = cmd.ExecuteNonQuery();

                    //MessageBox.Show("Records Deleted From Stop Details Table : " + rowsModified);
                    //textBox1.Text = textBox1.Text + "\r\n" +  "Records Deleted From Stop Details Table : " + rowsModified;
                    listBox2.Items.Add("Records Deleted From Stop Details Table : " + rowsModified);
                 
                }
                catch (Exception StoDEx)
                {
                    tx.Rollback();
                    //textBox1.Text = textBox1.Text + "\r\n" + "Stops Details Could not be deleted. ";
                    MessageBox.Show("Stops Details Could not be deleted. due to {0}",StoDEx.Message);
                    return;
                }


                try {
                    sql = string.Format(@"DELETE FROM Lines_Info;");
                    cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql;
                    cmd.Transaction = tx;
                    int rowsModified = cmd.ExecuteNonQuery();

                    //MessageBox.Show("Records Deleted From Lines Table : " + rowsModified);
                   // textBox1.Text = textBox1.Text + "\r\n" + "Records Deleted From Lines Table : " + rowsModified;
                    listBox2.Items.Add("Records Deleted From Lines Table : " + rowsModified);
                 
                }
                catch (Exception LEX)
                {
                    tx.Rollback();
                    //textBox1.Text = textBox1.Text + "\r\n" +  "Stations Could not be deleted. ";
                    MessageBox.Show("Stations Could not be deleted due to : {0}", LEX.Message);
                    return;
                }

                try {
                    sql = string.Format(@"DELETE FROM Stops;");
                    cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql;
                    cmd.Transaction = tx;
                    int rowsModified = cmd.ExecuteNonQuery();

                    //MessageBox.Show("Records Deleted From Stop Table : " + rowsModified);
                    //textBox1.Text = textBox1.Text + "\r\n" +"Records Deleted From Stop Table : " + rowsModified;
                    listBox2.Items.Add("Records Deleted From Stop Table : " + rowsModified);
                 
                }
                catch (Exception Stops)
                {
                    tx.Rollback();
                    MessageBox.Show("Stops Table Could not be deleted. Exception : {0}",Stops.Message);
                    return;
                }

                try {
                    sql = string.Format(@"DELETE FROM Station_Info;");
                    cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql;
                    cmd.Transaction = tx;
                    int rowsModified = cmd.ExecuteNonQuery();

                    //MessageBox.Show("Records Deleted From Stations Table : " + rowsModified);
                    //textBox1.Text = textBox1.Text + "\r\n" + "Records Deleted From Stations Table : " + rowsModified;
                    listBox2.Items.Add("Records Deleted From Stations Table : " + rowsModified);
                 
                }
                catch (Exception StationEx)
                {
                    tx.Rollback();
                    MessageBox.Show("Lines Table Could not be deleted.{0}", StationEx.Message);
                    return;
                }
                tx.Commit();
            }
            catch(Exception Ex)
            {
                tx.Rollback();
                MessageBox.Show("Purge of the database failed!! Try Manually : {0}", Ex.Message);
                return;
            }
            finally
            {
                db.Close();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlConnection db;
            SqlCommand cmd;
            string sql;
            object result, result2;

            try
            {
                db = new SqlConnection(connectionInfo);
                db.Open();
            }
            catch (Exception Ex)
            {
                string msg2 = string.Format("Open Failed,'{0}'", Ex.Message);
                MessageBox.Show(msg2);
                return;
            }

            string listbox1 = this.listBox1.Text;

           // string Direction = 
            //string Handicap = this.textBox6.Text;
            //string StationName = this.textBox3.Text;

            listbox1 = listbox1.Replace("'", "''");

            string[] station_details = listbox1.Split('|');

            try {
                sql = string.Format(@"
                                SELECT Direction,
		                                (CASE WHEN 
			                                Handicap_Accessible = 'false'
			                                THEN 'No'
		                                WHEN UPPER(Handicap_Accessible) = UPPER('True')
		                                   THEN 'Yes'
		                                ELSE 'N/A'
		                                END) Handicap,
		                                (SELECT Station_Name
		                                   FROM Station_Info
		                                   WHERE Station_id = S.Station_id
		                                ) Station_Name,
                                           Stop_ID
                                  FROM Stops S
                                 WHERE Stop_ID = '{0}';",station_details[0]);

                cmd = new SqlCommand();
                cmd.Connection = db;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();

                cmd.CommandText = sql;
                adapter.Fill(ds);

                DataTable dt = ds.Tables["TABLE"];

                if (dt.Rows.Count != 1)
                {
                    MessageBox.Show("Did not find information on this stop");
                    return;
                }

                DataRow R = dt.Rows[0];
                //Direction
                this.textBox2.Text = R["Direction"].ToString();
                //Handicap
                this.textBox6.Text = R["Handicap"].ToString();
                // Station Name
                this.textBox3.Text = R["Station_Name"].ToString();
                string stop_id = R["Stop_ID"].ToString();
                string station_name = this.textBox3.Text;
                station_name = station_name.Replace("'", "''");
                
                //MessageBox.Show("Station Name " + station_name + "stop_id " + stop_id);
                
                sql = string.Format(@" 
                        SELECT SUM(R.rides) SUM_STATION_RIDE, 
		                        AVG(R.rides) AVG_STATION_RIDE
                           FROM Riderships R
                          WHERE station_id = (SELECT DISTINCT S.station_id 
						                        FROM Station_Info S, 
							                         Stops S1
					                           WHERE Station_Name = '{0}'
					                             AND S.Station_id = S1.Station_id
						                         AND S1.Stop_id = '{1}')
                           ;", station_name, stop_id);

                cmd = new SqlCommand();
                cmd.Connection = db;
                cmd.CommandText = sql;
                adapter = new SqlDataAdapter(cmd);
                ds = new DataSet();

                adapter.Fill(ds);


                dt = ds.Tables["TABLE"];

                if (dt.Rows.Count != 1)
                {
                    MessageBox.Show("Could not Look up Ridership Stats Details");
                }

                DataRow R1 = dt.Rows[0];
                //SUM FOR STATION
                this.textBox8.Text = R1["SUM_STATION_RIDE"].ToString();
                //AVERAGE FOR STATION
                this.textBox9.Text = R1["AVG_STATION_RIDE"].ToString();

                sql = string.Format(@" 
                           SELECT Line_Name 
                             FROM Lines_Info
                            WHERE line_id IN 
	                        (SELECT Line_id
	                          FROM Stop_Details  
	                          WHERE Stop_id = '{0}');", stop_id);

                cmd = new SqlCommand();
                cmd.Connection = db;
                cmd.CommandText = sql;
                adapter = new SqlDataAdapter(cmd);
                ds = new DataSet();

                adapter.Fill(ds);


                dt = ds.Tables["TABLE"];

                //MessageBox.Show("Row Count :" + dt.Rows.Count);
                if (dt.Rows.Count < 1)
                {
                    MessageBox.Show("No Line Information present for this stop!!");
                }

                string Line_Name = "";
                foreach (DataRow row in dt.Rows)
                {
                    if (Line_Name != "")
                        Line_Name += ", ";
                    //MessageBox.Show("Line Name :" + Line_Name);
                    Line_Name += row["Line_Name"].ToString();
                }

                this.textBox4.Text = Line_Name;

            }
            catch(Exception Ex) { 
                MessageBox.Show("Error Occured while getting Stop Details :" + Ex.Message);
            }
            finally
            {
                db.Close();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SqlConnection db;

            SqlCommand cmd;
            SqlCommand cmd2;
            SqlCommand cmd3;

            bool done = false;
            string checksql, sql, sql2, msg;
           
            //SqlTransaction tx = null;
           
           
           try
            {
                db = new SqlConnection(connectionInfo);
                db.Open();
            }
            catch(Exception Ex){
                string msg2 = string.Format("Open Failed,'{0}'", Ex.Message);
                MessageBox.Show(msg2);
                return;
            }

           try
           {
                cmd = new SqlCommand();
                cmd.Connection = db;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Stop_Name";

                adapter.Fill(ds);

                DataTable dt = ds.Tables["TABLE"];
                foreach (DataRow row in dt.Rows)
                {

                    msg = string.Format("{0}",
                    row["Stop_Name"].ToString()
                    );
                    //MessageBox.Show(msg);
                    this.listBox1.Items.Add(msg);
                }
            }
            catch(Exception Inner){
                MessageBox.Show("Error Occured :" + Inner.Message);
                return;
            }
           try 
           { 
                cmd = new SqlCommand();
                cmd2 = new SqlCommand();
                cmd3 = new SqlCommand();
               /////
                cmd.Connection = db;
                
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                SqlDataAdapter adapter2 = new SqlDataAdapter(cmd2);
                SqlDataAdapter adapter3 = new SqlDataAdapter(cmd3);
                //
                DataSet ds = new DataSet();
                DataSet ds2 = new DataSet();
                DataSet ds3 = new DataSet();
                //
                string daytype = null;
                string daytype2 = null;
                string daytype3 = null;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Average_Station";

                var fn = cmd.Parameters.Add("daytype", SqlDbType.NVarChar);
                fn.Direction = ParameterDirection.Input;

                daytype = "W";
                daytype2 = "A";
                daytype3 = "U";

                fn.Value = daytype;


                adapter.Fill(ds);

                DataTable dt = ds.Tables["TABLE"];
               
                foreach (DataRow row in dt.Rows)
                {
                    msg = string.Format("{0} : {1}",
                    row["station_name"].ToString(),
                    row["Max_avg"].ToString()
                    );
                    //MessageBox.Show(msg);
                    textBox10.Text = msg;
                }

                cmd2.Connection = db;
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.CommandText = "Average_Station";

                var fn1 = cmd2.Parameters.Add("daytype", SqlDbType.NVarChar);
                fn1.Direction = ParameterDirection.Input;
                
                fn1.Value = daytype2;

                adapter2.Fill(ds2);

                DataTable dt2 = ds2.Tables["TABLE"];

                foreach (DataRow row in dt2.Rows)
                {
                    msg = string.Format("{0} : {1}",
                    row["station_name"].ToString(),
                    row["Max_avg"].ToString()
                    );
                    //MessageBox.Show(msg);
                    textBox11.Text = msg;
                }



                cmd3.Connection = db;
                cmd3.CommandType = CommandType.StoredProcedure;
                cmd3.CommandText = "Average_Station";

                var fn2 = cmd3.Parameters.Add("daytype", SqlDbType.NVarChar);
                fn2.Direction = ParameterDirection.Input;
                fn2.Value = daytype3;

                adapter3.Fill(ds3);

                DataTable dt3 = ds3.Tables["TABLE"];

                foreach (DataRow row in dt3.Rows)
                {
                    msg = string.Format("{0} : {1}",
                    row["station_name"].ToString(),
                    row["Max_avg"].ToString()
                    );
                    //MessageBox.Show(msg);
                    textBox12.Text = msg;
                }
           }
           catch (Exception Ex)
           {
               MessageBox.Show("Error occured :" + Ex.Message);
           }
           finally
           {
               db.Close();
           }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
