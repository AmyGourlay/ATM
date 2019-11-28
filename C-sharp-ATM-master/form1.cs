using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class ATM : Form
    {


        //local referance to the array of accounts
        private Account[] ac;

        private Thread atm1_t, atm2_t;

        //this is a referance to the account that is being used
        private Account activeAccount = null;

        private bool accountOpen = false, pinVerified = false, withdrawOpen = false;

        int pinAttempts = 0;

        //used to stop the form from locking while waiting for enter to be pressed
        private readonly ManualResetEvent enterPressed = new ManualResetEvent(false);

        // the atm constructor takes an array of account objects as a referance
        public ATM(Account[] ac)
        {
            InitializeComponent();

            ThreadStart startThread1 = new ThreadStart(atm1);
            atm1_t = new Thread(startThread1);
            ThreadStart startThread2 = new ThreadStart(atm2);
            atm2_t = new Thread(startThread2);

            new ATM();
            new ATM();

            this.ac = ac;
            lbl1.Text = "";
            lbl2.Text = "";
            lbl3.Text = "enter your account number..";
        }

        private void atm1()
        {
            

        }

        private void atm2()
        {
            

        }

        private void atmLoop()
        {
            // an infanite loop to keep the flow of controll going on and on
            while (true)
            {

                //ask for account number and store result in acctiveAccount (null if no match found)
                activeAccount = this.findAccount();

                if (activeAccount != null)
                {
                    //if the account is found check the pin 
                    if (activeAccount.checkPin(this.promptForPin()))
                    {
                        //if the pin is a match give the options to do stuff to the account (take money out, view balance, exit)
                        dispOptions();
                    }
                }
                else
                {   //if the account number entered is not found let the user know!
                    lbl3.Text = "no matching account found.";
                }

                //wipes all text from the console
                lbl1.Text = "";
                lbl2.Text = "";
                lbl3.Text = "";
            }
        }

        /*
         *    this method promts for the input of an account number
         *    the string input is then converted to an int
         *    a for loop is used to check the enterd account number
         *    against those held in the account array
         *    if a match is found a referance to the match is returned
         *    if the for loop completest with no match we return null
         * 
         */
        private Account findAccount()
        {
            
            int input = Convert.ToInt32(txtInput.Text);
            txtInput.Clear();
            for (int i = 0; i < this.ac.Length; i++)
            {
                if (ac[i].getAccountNum() == input)
                {
                    return ac[i];
                }
            }

            return null;
        }
        /*
         * 
         *  this jsut promt the use to enter a pin number
         *  
         * returns the string entered converted to an int
         * 
         */
        private int promptForPin()
        {
            String str = txtInput.Text;
            int pinNumEntered = Convert.ToInt32(str);
            txtInput.Clear();
            return pinNumEntered;
        }

        /*
         * 
         *  give the use the options to do with the accoutn
         *  
         *  promt for input
         *  and defer to appropriate method based on input
         *  
         */
        private void dispOptions()
        {
            lbl1.Text = "1> take out cash";
            lbl2.Text = "2> balance";
            lbl3.Text = "3> exit";

            int input = Convert.ToInt32(Console.ReadLine());

            if (input == 1)
            {
                dispWithdraw();
            }
            else if (input == 2)
            {
                dispBalance();
            }
            else if (input == 3)
            {


            }
            else
            {

            }

        }

        /*
         * 
         * offer withdrawable amounts
         * 
         * based on input attempt to withraw the corosponding amount of money
         * 
         */
        private void dispWithdraw()
        {
            lbl1.Text = "1> 10";
            lbl2.Text = "2> 50";
            lbl3.Text = "3> 500";

            int input = Convert.ToInt32(Console.ReadLine());

            if (input > 0 && input < 4)
            {

                //opiton one is entered by the user
                if (input == 1)
                {

                    //attempt to decrement account by 10 punds
                    if (activeAccount.decrementBalance(10))
                    {
                        //if this is possible display new balance and await key press
                        lbl1.Text = "new balance " + activeAccount.getBalance();
                        lbl2.Text = " (prese enter to continue)";
                        enterPressed.WaitOne();
                    }
                    else
                    {
                        //if this is not possible inform user and await key press
                        lbl1.Text = "insufficent funds";
                        lbl2.Text = " (prese enter to continue)";
                        enterPressed.WaitOne();
                    }
                }
                else if (input == 2)
                {
                    if (activeAccount.decrementBalance(50))
                    {
                        lbl1.Text = "new balance " + activeAccount.getBalance();
                        lbl2.Text = " (prese enter to continue)";
                        enterPressed.WaitOne();
                    }
                    else
                    {
                        lbl1.Text = "insufficent funds";
                        lbl2.Text = " (prese enter to continue)";
                        enterPressed.WaitOne();
                    }
                }
                else if (input == 3)
                {
                    if (activeAccount.decrementBalance(500))
                    {
                        lbl1.Text = "new balance " + activeAccount.getBalance();
                        lbl2.Text = " (prese enter to continue)";
                        enterPressed.WaitOne();
                    }
                    else
                    {
                        lbl1.Text = "insufficent funds";
                        lbl2.Text = " (prese enter to continue)";
                        enterPressed.WaitOne();
                    }
                }
            }
        }
        /*
         *  display balance of activeAccount and await keypress
         *  
         */
        private void dispBalance()
        {
            if (this.activeAccount != null)
            {
                lbl1.Text = " your current balance is : " + activeAccount.getBalance();
                lbl2.Text = " (prese enter to continue)";
                enterPressed.WaitOne();
            }
        }

        private void optionBtn_Click(object sender, EventArgs e)
        {
            if (pinVerified)
            {
                if (withdrawOpen)
                {

                }
            }
        }

        private void ATM_Load(object sender, EventArgs e)
        {

        }

        public ATM()
        {
            InitializeComponent();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (activeAccount!=null)
            {
                if (pinVerified)
                {
                    enterPressed.Set();
                }
                else
                {
                    int pin = promptForPin();
                    if (activeAccount.checkPin(pin))
                    {
                        pinVerified = true;
                        dispOptions();
                    }
                    else
                    {
                        lbl2.Text = "Wrong Pin";
                        pinAttempts++;
                        if(pinAttempts >= 3)
                        {
                            activeAccount = null;
                            lbl2.Text = "";
                            lbl3.Text = "enter your account number..";
                        }
                    }
                }
            }
            else
            {
                Account acc = findAccount();
                if(acc != null)
                {
                    activeAccount = acc;
                    pinAttempts = 0;
                    pinVerified = false;
                    lbl3.Text = "enter pin:";
                }
            }
        }
    }
}


