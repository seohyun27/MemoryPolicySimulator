import javax.swing.*;
import java.awt.*;
import java.util.List;
import java.util.Random;

public class GUI extends JFrame {
    private Core core;
    private JTextField inputField;
    private JTextField frameSizeField; // 프레임 크기 입력 칸
    private JComboBox<String> policyComboBox;
    private JTextArea historyArea;
    private JTextArea infoArea;
    private JButton runButton;
    private JButton randomButton;

    public GUI() {
        // Core 인스턴스 생성 (기본 frame 4, 기본 policy FIFO)
        core = new Core(4, 'F');

        setTitle("Memory Policy Simulator");
        setSize(800, 600);
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setLocationRelativeTo(null);

        // 상단 입력 패널
        JPanel topPanel = new JPanel();
        topPanel.setLayout(new FlowLayout());

        inputField = new JTextField(15);
        frameSizeField = new JTextField("4", 3);
        policyComboBox = new JComboBox<>(new String[]{"FIFO", "LFU", "MFU", "PFU"});
        runButton = new JButton("Run");
        randomButton = new JButton("Random");

        topPanel.add(new JLabel("Input String:"));
        topPanel.add(inputField);
        topPanel.add(new JLabel("Frame Size:"));
        topPanel.add(frameSizeField);
        topPanel.add(new JLabel("Policy:"));
        topPanel.add(policyComboBox);
        topPanel.add(runButton);
        topPanel.add(randomButton);

        // 중앙 영역: History와 Info
        JSplitPane splitPane = new JSplitPane(JSplitPane.HORIZONTAL_SPLIT);
        historyArea = new JTextArea();
        historyArea.setEditable(false);
        historyArea.setBorder(BorderFactory.createTitledBorder("History"));
        JScrollPane historyScroll = new JScrollPane(historyArea);

        infoArea = new JTextArea();
        infoArea.setEditable(false);
        infoArea.setBorder(BorderFactory.createTitledBorder("Statistics"));
        JScrollPane infoScroll = new JScrollPane(infoArea);

        Font font = new Font("Serif", Font.PLAIN, 16);
        historyArea.setFont(font);
        infoArea.setFont(font);

        splitPane.setLeftComponent(historyScroll);
        splitPane.setRightComponent(infoScroll);

        splitPane.setDividerLocation(0.6);
        splitPane.setResizeWeight(0.6);

        // 레이아웃 구성
        setLayout(new BorderLayout());
        add(topPanel, BorderLayout.NORTH);
        add(splitPane, BorderLayout.CENTER);

        // 버튼 이벤트
        runButton.addActionListener(e -> runSimulation());
        randomButton.addActionListener(e -> generateRandomString());
    }

    private void runSimulation() { // 페이지 시뮬레이터를 실행하는 함수
        String inputStr = inputField.getText().trim();
        String frameSizeText = frameSizeField.getText().trim();

        // 프레임 크기 유효성 체크, 유효하지 않다면 그대로 리턴
        int frameSize;
        try {
            frameSize = Integer.parseInt(frameSizeText);
            if (frameSize <= 0) { // 프레임 크기가 0 이하라면
                return;
            }
        } catch (NumberFormatException e) { // 프레임 크기가 숫자가 아니라면
            return;
        }

        if (inputStr.isEmpty()) { //문자열이 비어있다면
            return;
        }

        // 정책 선택
        String policyStr = (String) policyComboBox.getSelectedItem();
        char policyChar = 'F'; // 기본 FIFO
        if (policyStr.equals("FIFO")) {
            policyChar = 'F';
        } else if (policyStr.equals("LFU")) {
            policyChar = 'L';
        } else if (policyStr.equals("MFU")) {
            policyChar = 'M';
        } else if (policyStr.equals("PFU")) { //내 정책
            policyChar = 'P';
        }

        core = new Core(frameSize, policyChar);

        // 입력 문자열의 글자 하나하나에 대해 operate() 함수 실행
        for (int i = 0; i < inputStr.length(); i++) {
            char ch = inputStr.charAt(i);
            core.operate(ch);
        }

        // History 출력
        StringBuilder historyBuilder = new StringBuilder();
        for (String line : core.History) {
            historyBuilder.append(line).append("\n");
        }
        historyArea.setText(historyBuilder.toString());

        // 통계 정보 출력
        List<String> info = core.getInfo();
        StringBuilder infoBuilder = new StringBuilder();
        for (String line : info) {
            infoBuilder.append(line).append("\n");
        }
        infoArea.setText(infoBuilder.toString());
    }

    private void generateRandomString() { //랜덤 스트링을 만드는 함수
        StringBuilder sb = new StringBuilder();
        Random rand = new Random();
        for (int i = 0; i < 50; i++) {
            char c = (char) ('A' + rand.nextInt(26));
            sb.append(c);
        }
        inputField.setText(sb.toString());
    }

    public static void main(String[] args) {
        SwingUtilities.invokeLater(() -> {
            GUI gui = new GUI();
            gui.setVisible(true);
        });
    }
}
