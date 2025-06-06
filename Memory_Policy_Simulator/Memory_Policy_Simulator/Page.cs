using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator
{
    public class Page
    {
        public static int CREATE_ID = 0;

        public enum STATUS
        {
            HIT,
            PAGEFAULT,
            MIGRATION
        }

        public int pid;
        public int loc;
        public char data;
        public STATUS status;
        public int Count; //참조 횟수를 세기 위한 변수
    }

}
