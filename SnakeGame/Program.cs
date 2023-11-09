namespace SnakeGame
{
    internal class Programs
    {


        static void Main(string[] args)
        {


            GameManager gameManager = new GameManager();
            gameManager.GameStart();

            System.Console.WriteLine("패배");

            return;
        }
    }
}



public class GameManager
{
    char[,] gridField = new char[20, 20];
    List<Point> m_snake = new List<Point>();
    Point m_food;
    bool hasFood = false;
    Direction direction;

    public struct Point
    {
        public int x;
        public int y;
    }

    public GameManager()
    {
        //처음 이동방향을 오른쪽으로 초기화
        direction = Direction.RIGHT;

        //기본 생성자에서 게임판 테두리를 미리 그려놓기
        for (int i = 1; i < 19; i++)
        {
            gridField[0, i] = 'ㅡ';
            gridField[19, i] = 'ㅡ';
            gridField[i, 0] = '│';
            gridField[i, 19] = '│';
        }

        gridField[0, 0] = '┌';
        gridField[0, 19] = '┐';
        gridField[19, 0] = '└';
        gridField[19, 19] = '┘';


        //뱀 처음 시작포인트 생성
        Point tmp_snake = new Point();
        tmp_snake.x = 10;
        tmp_snake.y = 10;
        m_snake.Add(tmp_snake);

        DrawSnake();
        DrawFood();
    }

    private void DrawSnake()
    {
        //게임판 내부를 초기화
        for (int i = 1; i < 19; i++)
        {
            for (int j = 1; j < 19; j++)
            {
                gridField[i, j] = 'ㅤ';
            }
        }

        //뱀 그리기
        for (int i = 0; i < m_snake.Count; i++)
        {
            int x = m_snake[i].x;
            int y = m_snake[i].y;


            //머리부분만 다르게 그리기
            if (i == 0)
            {
                gridField[x, y] = '□';
            }
            else
            {
                gridField[x, y] = '■';
            }
        }
    }

    //뱀을 이동시키는 함수
    private void MoveSnake()
    {
        //리스트 구조체안의 변수를 직접 수정할수 없으므로 임시 구조체로 덮어씌운다.
        Point tmp_point = new Point();
        tmp_point.x = m_snake[0].x;
        tmp_point.y = m_snake[0].y;


        //xy 서로 반대로 바뀌었다고 생각한다
        //현재 이동방향에 따라 뱀이동
        switch (direction)
        {
            case Direction.RIGHT:
                tmp_point.y += 1;
                break;
            case Direction.UP:
                tmp_point.x -= 1;
                break;
            case Direction.DOWN:
                tmp_point.x += 1;
                break;
            case Direction.LEFT:
                tmp_point.y -= 1;
                break;

        }

        //뱀머리와 밥이 겹쳤을때 뱀 길이 늘리기
        if (m_food.x == tmp_point.x && m_food.y == tmp_point.y)
        {
            Point tmp_point2 = new Point();
            tmp_point2.x = m_snake[m_snake.Count - 1].x;
            tmp_point2.y = m_snake[m_snake.Count - 1].y;

            m_snake.Add(tmp_point2);

            hasFood = false;
        }

        if (m_snake.Count > 1)
        {
            //머리빼고 리스트 순서대로 좌표값 전달
            for (int i = m_snake.Count - 1; i > 0; i--) 
            {
                m_snake[i] = m_snake[i - 1];
            }
        }

        //머리부분 이동
        m_snake[0] = tmp_point;

    }

    private void DrawFood()
    {

        //현재 먹이가 없을때만 생성
        if (!hasFood)
        {

            while (true)
            {
                int x = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, 18);
                int y = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, 18);

                //먹이를 둘 자리에 무언가 있으면 다시 처음으로 돌아가서 랜덤 좌표 생성
                if (gridField[x, y] == 'ㅤ')
                {
                    m_food.x = x;
                    m_food.y = y;
                    hasFood = true;
                    break;
                }
                else
                {
                    continue;
                }

            }
        }
        else
        {
            //먹이는 다른 문자로 표현
            gridField[m_food.x, m_food.y] = '♥';
        }

    }

    private bool CheckCollision()
    {
        //맵밖으로 나갔을 경우
        if (m_snake[0].x > 18 || m_snake[0].x < 1 || m_snake[0].y > 18 || m_snake[0].y < 1)
        {
            return true;
        }

        //자기 몸이랑 충돌햇을 경우
        for (int i = 1; i < m_snake.Count; i++)
        {
            if (m_snake[0].x == m_snake[i].x && m_snake[0].y == m_snake[i].y)
            {
                return true;
            }

        }

        return false;
    }

    public void InputKey()
    {
        ConsoleKeyInfo m_key;

        while (true)
        {
            m_key = Console.ReadKey(true);

            switch (m_key.Key)
            {
                case ConsoleKey.LeftArrow:
                    direction = Direction.LEFT;

                    break;
                case ConsoleKey.RightArrow:
                    direction = Direction.RIGHT;

                    break;
                case ConsoleKey.UpArrow:
                    direction = Direction.UP;

                    break;
                case ConsoleKey.DownArrow:
                    direction = Direction.DOWN;

                    break;
            }

        }

    }

    //게임시작 함수
    public void GameStart()
    {
        bool result;

        while (true)
        {
            Thread keyCheck = new Thread(InputKey);
            DrawSnake();
            DrawFood();

            //키입력 함수는 다른 스레드로 돌린다.
            keyCheck.Start();

            //렌더링
            System.Console.Clear();

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    System.Console.Write(gridField[i, j]);
                }
                System.Console.WriteLine();
            }

            Thread.Sleep(100);

            //현재 이동방향에 따라 뱀이동
            MoveSnake();

            //충돌체크
            result = CheckCollision();
            if (result)
            {
                //뱀이 벽이나 자기 몸에 충돌했을때 메모리 반환 후 게임 종료
                GameOver();

                return;
            }
        }
    }

    //메모리 반환 함수
    public void GameOver()
    {

    }
}

public enum Direction
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}