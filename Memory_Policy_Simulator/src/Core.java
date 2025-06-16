import java.util.ArrayList;
import java.util.List;
import java.util.Random;

public class Core {
    private int cursor;
    public int p_frame_size;
    public List<Page> frame_window;
    public List<Page> pageHistory;
    public List<String> History;
    private char memoryPolicy; // 정책
    private static final int MAX_COUNT = 10; // 한 페이지의 최대 참조 횟수
    private static final double PFU_PROBABILITY = 0.5; // PFU의 인덱스 선택 확률

    public int hit;
    public int fault;
    public int migration;

    public Core(int get_frame_size, char memoryPolicy) {
        this.cursor = 0;
        this.p_frame_size = get_frame_size;
        this.frame_window = new ArrayList<>();
        this.pageHistory = new ArrayList<>(); // 입력된 페이지를 순서대로 저장하기 위한 배열
        this.History = new ArrayList<>(); // 현재 상태를 백업해 문자열로 저장
        this.memoryPolicy = memoryPolicy; // 정책 선택
        this.hit = 0;
        this.fault = 0;
        this.migration = 0;

        History.add("Pgae        frame_window");
    }

    public Page.STATUS operate(char data) {
        Page newPage = new Page();
        StringBuilder sb = new StringBuilder();

        boolean hitPage = frame_window.stream().anyMatch(x -> x.data == data);
        if (hitPage) {
            // 페이지 히트
            newPage.pid = Page.CREATE_ID++;
            newPage.data = data;
            newPage.status = Page.STATUS.HIT;
            this.hit++;

            int i = 0;
            for (; i < frame_window.size(); i++) {
                if (frame_window.get(i).data == data) break;
            }
            newPage.loc = i + 1;

            if (frame_window.get(i).count <= MAX_COUNT) {
                frame_window.get(i).count++;
            }
            newPage.count = frame_window.get(i).count;
        } else {
            // 교체 또는 fault
            newPage.pid = Page.CREATE_ID++;
            newPage.data = data;
            newPage.count = 0; // 새 페이지는 참조 횟수를 0으로 초기화

            if (frame_window.size() >= p_frame_size) {
                // 페이지 교체
                newPage.status = Page.STATUS.MIGRATION;

                if (this.memoryPolicy == 'F') { // FIFO 정책
                    frame_window.remove(0);
                    newPage.loc = cursor;
                } else if (this.memoryPolicy == 'L') { // LFU 정책
                    int minIndex = 0;
                    for (int i = 1; i < frame_window.size(); i++) {
                        if (frame_window.get(i).count < frame_window.get(minIndex).count) {
                            minIndex = i;
                        }
                    }
                    frame_window.remove(minIndex);
                    newPage.loc = minIndex + 1;
                } else if (this.memoryPolicy == 'M') { // LFU 정책
                    int maxIndex = 0;
                    for (int i = 1; i < frame_window.size(); i++) {
                        if (frame_window.get(i).count > frame_window.get(maxIndex).count) {
                            maxIndex = i;
                        }
                    }
                    frame_window.remove(maxIndex);
                    newPage.loc = maxIndex + 1;
                }  else if (this.memoryPolicy == 'P') { // PFU 정책
                    int minIndex = 0;
                    int maxIndex = 0;
                    for (int i = 1; i < frame_window.size(); i++) {
                        if (frame_window.get(i).count < frame_window.get(minIndex).count) {
                            minIndex = i;
                        }
                        if (frame_window.get(i).count > frame_window.get(maxIndex).count) {
                            maxIndex = i;
                        }
                    }

                    // minIndex, maxIndex 중 하나를 랜덤 선택
                    Random rand = new Random();
                    double randProb = rand.nextDouble();
                    int chooseIndex;
                    if (randProb < PFU_PROBABILITY) {
                        chooseIndex = minIndex;
                    } else {
                        chooseIndex = maxIndex;
                    }

                    frame_window.remove(chooseIndex);
                    newPage.loc = chooseIndex + 1;
                }

                cursor = p_frame_size;
                frame_window.add(newPage);
                this.migration++;
                this.fault++;
            } else {
                // 페이지 fault
                newPage.status = Page.STATUS.PAGEFAULT;
                cursor++;
                this.fault++;
                newPage.loc = cursor;
                frame_window.add(newPage);
            }
        }

        // pageHistory 기록
        pageHistory.add(newPage);

        // History 기록
        sb.append(newPage.data + "       :    ");

        for (Page page : frame_window) { // 프레임 윈도우 상태 백업
            sb.append(page.data + " ");
        }

        String status;
        switch (newPage.status){
            case Page.STATUS.HIT :
                status = "(Hit)";
                break;
            case Page.STATUS.MIGRATION :
                status = "(Migration)";
                break;
            case Page.STATUS.PAGEFAULT :
                status = "(Fault)";
                break;
            default :
                status = "";
        }

        sb.append(status);
        String result = sb.toString();
        History.add(result);

        return newPage.status;
    }

    public List<String> getInfo() { // 통계 정보 리턴
        List<String> info = new ArrayList<>();

        StringBuilder sb = new StringBuilder();
        sb.append("Hit 횟수 : ");
        sb.append(this.hit);
        String result = sb.toString();
        info.add(result);
        sb.setLength(0);

        sb.append("Migration 횟수 : ");
        sb.append(this.migration);
        result = sb.toString();
        info.add(result);
        sb.setLength(0);

        sb.append("fault 횟수 : ");
        sb.append(this.fault);
        result = sb.toString();
        info.add(result);
        sb.setLength(0);

        info.add("");

        // fault_rate 계산
        double fault_rate = (double) this.fault / (this.fault + this.hit) * 100;
        fault_rate = Math.round(fault_rate * 100.0) / 100.0;
        sb.append("fault rate : ");
        sb.append(fault_rate);
        sb.append("%");
        result = sb.toString();
        info.add(result);

        return info;
    }
}
