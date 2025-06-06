using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Memory_Policy_Simulator
{ //Core 클래스: 페이지 교체 정책(예: FIFO, LRU 등)을 시뮬레이션하는 핵심 로직. // 페이지 요청을 받고, 페이지 교체 정책에 따라 페이지를 관리하며, 페이지 히스토리와 통계(히트, 페이지 폴트 등)를 기록.

    class Core
    {
        private int cursor; //현재 페이지 위치를 추적(1~p_frame_size)
        public int p_frame_size; //프레임 크기 (메모리에 올라가는 최대 페이지의 수)
        public List<Page> frame_window; //메모리 (현재 메모리 위에 올라가 있는 페이지들을 관리)
        public List<Page> pageHistory; //모든 페이지 접근 이력
        private char memoryPolicy; //정책

        private int pointer; //페이지 위치를 표시하는 포인터(0~p_frame_size-1)

        //성능을 판단하기 위한 지표
        public int hit; //히트 횟수
        public int fault; //fault 횟수
        public int migration; //페이지 교체 횟수

        public Core(int get_frame_size, char memoryPolicy) //생성자(프레임 크기, 정책)
        {
            this.cursor = 0;
            this.p_frame_size = get_frame_size;
            this.frame_window = new List<Page>(); //메모리 생성
            this.pageHistory = new List<Page>(); //페이지 접근 이력 생성
            this.memoryPolicy = memoryPolicy; //내가 추가함
            this.pointer = 0;
        }

        public Page.STATUS Operate(char data)
        {
            Page newPage = new Page();

            //만약 메모리에 페이지가 있다면 -> 페이지 히트
            if (this.frame_window.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.ReferBit = true; //참조 비트 1로 설정
                newPage.status = Page.STATUS.HIT; //페이지 상태 = 히트
                this.hit++;

                int i;
                i = this.frame_window.FindIndex(x => x.data == data);
                this.frame_window[i].ReferBit = true; //메모리 위 page의 참조 비트를 1로 설정
                newPage.loc = i + 1;
            }
            else //메모리에 페이지가 없다면 -> 교체 or 폴트
            {
                newPage.pid = Page.CREATE_ID++; //새 페이지 만들기
                newPage.data = data;
                newPage.ReferBit = true; //참조 비트 1로 설정

                if (frame_window.Count >= p_frame_size) // 메모리가 가득 참 -> 페이지 migration(교체)
                {
                    newPage.status = Page.STATUS.MIGRATION; //페이지 상태 = 폴트

                    // ========== 정책별 분기 ==========
                    // 각 분기에서 '페이지 삭제/다시 삽입/cursor 변수의 값'만 조작할 것
                    if (memoryPolicy == 'F') //선택된 정책이 FIFO라면
                    {
                        this.frame_window.RemoveAt(0); //가장 먼저 들어온 페이지 제거
                        this.frame_window.Add(newPage); //큐의 뒤에 새 페이지를 추가
                        cursor = p_frame_size; //현재 페이지의 위치
                    }
                    else if (memoryPolicy == 'C') //선택된 정책이 Clock(Second Chance)이라면
                    {
                        //예시 1
                    }
                    else if (memoryPolicy == 'L') //선택된 정책이 LFU라면
                    {
                        // 예시2
                    }
                    else if (memoryPolicy == 'M')
                    {
                        // 예시3 -> 이건 내가 만든 정책
                    }

                    this.migration++; //교체 횟수 증가
                    this.fault++; //폴트 횟수 증가
                }
                else // 메모리에 페이지가 없고 빈 공간은 있음 -> 페이지 폴트
                {
                    newPage.status = Page.STATUS.PAGEFAULT; //페이지 상태 = 폴트
                    cursor++; //새 페이지의 위치
                    this.fault++; //폴트 횟수 증가
                    frame_window.Add(newPage); //메모리의 뒤에 새 페이지를 추가
                }

                newPage.loc = cursor; //새 페이지의 위치
            }
            pageHistory.Add(newPage); //페이지 이력 리스트에 새 페이지를 저장
            return newPage.status;
        }

        public List<Page> GetPageInfo(Page.STATUS status)
        {
            List<Page> pages = new List<Page>();

            foreach (Page page in pageHistory)
            {
                if (page.status == status)
                {
                    pages.Add(page);
                }
            }

            return pages;
        }

    }
}