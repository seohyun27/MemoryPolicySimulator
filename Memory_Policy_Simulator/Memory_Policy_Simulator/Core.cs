using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator
{

    //Core 클래스: 페이지 교체 정책(예: FIFO, LRU 등)을 시뮬레이션하는 핵심 로직.
    // 페이지 요청을 받고, 페이지 교체 정책에 따라 페이지를 관리하며, 페이지 히스토리와 통계(히트, 페이지 폴트 등)를 기록.
    class Core
    {
        private int cursor;
        public int p_frame_size;
        public Queue<Page> frame_window;
        public List<Page> pageHistory;

        public int hit;
        public int fault;
        public int migration;

        public Core(int get_frame_size)
        {
            this.cursor = 0;
            this.p_frame_size = get_frame_size;
            this.frame_window = new Queue<Page>();
            this.pageHistory = new List<Page>();
        }

        public Page.STATUS Operate(char data)
        {
            Page newPage;

            if (this.frame_window.Any<Page>(x => x.data == data))
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
                
            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;
                    this.frame_window.Dequeue();
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
                frame_window.Enqueue(newPage);
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

    }


}