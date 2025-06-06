using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics; //디버깅 용도

namespace Memory_Policy_Simulator
{
    class Core
    {
        private int cursor;
        public int p_frame_size;
        public List<Page> frame_window;
        public List<Page> pageHistory;
        private char memoryPolicy; //정책

        public int hit;
        public int fault;
        public int migration;

        public Core(int get_frame_size, char memoryPolicy)
        {
            this.cursor = 0;
            this.p_frame_size = get_frame_size;
            this.frame_window = new List<Page>();
            this.pageHistory = new List<Page>();
            this.memoryPolicy = memoryPolicy;
        }

        public Page.STATUS Operate(char data)
        {
            Page newPage = new Page();

            if (this.frame_window.Any<Page>(x => x.data == data)) //히트
            {                
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;
                this.hit++;
                int i;

                for (i = 0; i < this.frame_window.Count; i ++)
                {
                    if (this.frame_window.ElementAt(i).data == data) break;
                }
                newPage.loc = i+1;

                this.frame_window.ElementAt(i).Count++; //카운터의 증가는 잘 일어나고 있음. 문제 없다
                newPage.Count = this.frame_window.ElementAt(i).Count;
            }
            else //교체 or fault
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.Count = 0; // 새 페이지는 카운트 0으로 초기화

                //이 안에서 if문으로 각 정책 구별
                //삭제 파트만 각 정책 안에서 구현
                //hit, fault에서 count++ 구현할 것

                if (frame_window.Count >= p_frame_size)
                {
                    Debug.WriteLine($"start replacement");

                    newPage.status = Page.STATUS.MIGRATION;

                    if (this.memoryPolicy == 'F')
                    {
                        this.frame_window.RemoveAt(0);
                        Debug.WriteLine($"start FIFO");
                    }
                    else if (this.memoryPolicy == 'L')
                    {
                        Debug.WriteLine($"start LFU");

                        int i;
                        int min = 0;
                        for (i = 1; i < this.frame_window.Count; i++)
                        {
                            if (this.frame_window.ElementAt(i).Count < this.frame_window.ElementAt(min).Count) min = i;
                        }
                        // 삭제 대상 페이지 인덱스 min을 이용해서 페이지 객체를 찾기
                        var pageToRemove = this.frame_window[min];

                        this.frame_window.RemoveAt(min); //이것도 문제 없음
                    }

                    PrintFrameWindowStatus(); //디버깅용 코드


                    cursor = p_frame_size;
                    this.migration++;
                    this.fault++;
                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;
                    cursor++;
                    this.fault++;
                }

                newPage.loc = cursor;
                frame_window.Add(newPage);
            }
            pageHistory.Add(newPage);

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

        // 디버깅용 코드!!
        public void PrintFrameWindowStatus()
        {
            Debug.WriteLine("현재 프레임 윈도우 상태:");
            for (int i = 0; i < frame_window.Count; i++)
            {
                var page = frame_window[i];
                Debug.WriteLine($"Index: {i}, Data: {page.data}, Count: {page.Count}");
            }
            Debug.WriteLine("--------------------------------------------------");
        }

    }
}