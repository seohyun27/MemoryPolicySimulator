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
    private static final int MAX_COUNT = 10;
    private static final double PFU_PROBABILITY = 0.5;

    public int hit;
    public int fault;
    public int migration;

    public Core(int get_frame_size, char memoryPolicy) {
        this.cursor = 0;
        this.p_frame_size = get_frame_size;
        this.frame_window = new ArrayList<>();
        this.pageHistory = new ArrayList<>();
        this.History = new ArrayList<>();
        this.memoryPolicy = memoryPolicy;
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
            // 히트
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
            newPage.count = 0; // 새 페이지는 카운트 0으로 초기화

            if (frame_window.size() >= p_frame_size) {
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
                }  else if (this.memoryPolicy == 'P') {
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

                    // 랜덤 선택
                    Random rand = new Random();
                    double randProb = rand.nextDouble(); // 0.0 ~ 1.0 사이의 난수 생성
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
                // 프레임이 꽉 차지 않은 경우
                newPage.status = Page.STATUS.PAGEFAULT;
                cursor++;
                this.fault++;
                newPage.loc = cursor;
                frame_window.add(newPage);
            }
        }

        // 페이지 히스토리 기록
        pageHistory.add(newPage);

        sb.append(newPage.data + "       :    ");

        // 프레임 윈도우 상태 백업
        for (Page page : frame_window) {
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
        String result = sb.toString(); // 최종 문자열
        History.add(result);

        return newPage.status;
    }

    public List<String> getInfo() {
        List<String> info = new ArrayList<>();

        StringBuilder sb = new StringBuilder();
        sb.append("Hit 횟수 : ");
        sb.append(this.hit);
        String result = sb.toString();
        info.add(result);
        sb.setLength(0); // sb를 비우기

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

        double fault_rate = (double) this.fault / (this.fault + this.hit) * 100;
        fault_rate = Math.round(fault_rate * 100.0) / 100.0;
        sb.append("fault rate : ");
        sb.append(fault_rate);
        sb.append("%");
        result = sb.toString();
        info.add(result);

        return info;
    }

    public static void main(String[] args) {
        Core core = new Core(4, 'F');

        String string = "1112345161";
        for (int i = 0; i < string.length(); i++) {
            char ch = string.charAt(i);
            core.operate(ch);
        }

        for (String line : core.History) {
            System.out.println(line);
        }

        List<String> info = core.getInfo();
        for (String line : info) {
            System.out.println(line);
        }
    }
}
