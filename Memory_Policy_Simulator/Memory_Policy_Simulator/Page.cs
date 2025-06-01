using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator
{
    //Page 구조체: 페이지의 상태(HIT, PAGEFAULT, MIGRATION)와 위치, 데이터 등을 저장하는 데이터 구조.

    public struct Page
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
    }

}
