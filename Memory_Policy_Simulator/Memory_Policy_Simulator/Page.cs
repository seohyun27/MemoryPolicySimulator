using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator
{
    //Page 클래스: 페이지의 상태(HIT, PAGEFAULT, MIGRATION)와 위치, 데이터 등을 저장하는 데이터 구조.

    public class Page
    {
        //각 페이지에 고유한 ID를 부여하기 위한 정적 변수
        //페이지가 생성될 때마다 증가
        public static int CREATE_ID = 0; 

        public enum STATUS //페이지의 상태
        {
            HIT, //히트 (페이지가 이미 메모리에 존재)
            PAGEFAULT, //폴트 (페이지가 메모리에 존재하지 않음)
            MIGRATION //메모리에서 다른 페이지로 교체
        }

        public int pid; //페이지 아이디
        public int loc; //메모리 내부에서의 페이지 위치
        public char data; //페이지에 저장된 데이터
        public STATUS status; //페이지의 현재 상태
        public bool ReferBit; //페이지의 참조 비트(Clock 알고리즘에서 사용)
        public int ReferFrequency; //페이지의 참조 횟수(LFU 알고리즘에서 사용)
    }

}
