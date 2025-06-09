public class Page {
    public static int CREATE_ID = 0;

    public enum STATUS {
        HIT,
        PAGEFAULT,
        MIGRATION
    }

    public int pid;
    public int loc;
    public char data;
    public STATUS status;
    public int count; // 참조 횟수를 세기 위한 변수

    // 기본 생성자
    public Page() {
        this.pid = 0;
        this.loc = 0;
        this.data = '\0'; // 초기값
        this.status = STATUS.PAGEFAULT; // 기본값 설정
        this.count = 0;
    }
}
