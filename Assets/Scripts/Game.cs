using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Game : MonoBehaviour {

    private enum TileType {
        floor,
        ceiling,
        obstacle,
    }

    public AudioClip sfxGetReady;
    public AudioClip sfxGo;
    public AudioClip sfxJump;
    public AudioClip sfxObstacleSpawn;
    public AudioClip sfxDeathExplosion;
    public AudioClip sfxDeathScream;
    public AudioClip sfxOhNo;

    public Text scoreText;
    public Text readyText;
    public Text authorText;
    public Text titleText;
    public Text instructionText;
    public Text bestScoreText;

    //delay between the "front" of the level and the player's position
    public float levelConstructionDelay = 1f;
    //gameStartTime is the delay before the player actually starts controlling the character
    public float gameStartTime = 8f;
    public TwistLayout currentLayout;
    public Animator sunAnimator;

    private BestScoreKeeper bestScoreKeeper;
    private Dictionary<TileType, string> tilePrefabPaths;
    private int score = 0;
    private float maxCorridorHeight = 10f;
    private bool gameOver = false;

    private void Awake() {
        //init
        currentLayout = new TwistLayout();
        tilePrefabPaths = new Dictionary<TileType, string> {
            {TileType.floor, "Prefabs/FloorTile"},
            {TileType.ceiling, "Prefabs/CeilingTile"},
            {TileType.obstacle, "Prefabs/ObstacleTile"}
        };
        //have to find this every time because it's a persistent score keeper, can't bind it in the editor
        bestScoreKeeper = GameObject.Find("BestScoreKeeper").GetComponent<BestScoreKeeper>();

        //display the best score so far from the persistent score keeper
        bestScoreKeeper.UpdateBestScoreDisplay();

        audio.PlayOneShot(sfxGetReady);

        //UI stuff: go from "READY..." to "GO!" to no text
        StartCoroutine(EndIntroScreen());

        //the game starts fast to get through the initial spiral generation faster
        Time.timeScale = 4f;
        StartCoroutine(AdjustTimeScale(0.5f, 0.2f));

        //generate the level
        StartCoroutine(AddCorridorTiles(0.1f, 0.1f));
        StartCoroutine(AddObstacleTiles(10f, 1f));

        //start incrementing the score, it increases by 1 every time an obstacle is passed
        StartCoroutine(IncrementScore(gameStartTime + 3f, 1f));
    }

    //called by the player when a collision with a tile occurs
    public void GameOver(Collider2D deathTile) {
        //play some very important sound effects
        audio.PlayOneShot(sfxDeathExplosion);
        audio.PlayOneShot(sfxDeathScream);
        StartCoroutine(PlayOhNoSFX(1f));

        //stop incrementing score
        gameOver = true;

        //shake the screen, paint the colliding tile red
        Camera.main.GetComponent<CameraControl>().LaunchDeathCamera();
        deathTile.GetComponent<SpriteRenderer>().color = Color.red;

        //send the current score to the persistent best score keeper in case it's a new high score, then reload the game
        bestScoreKeeper.TryUpdateBestScore(score);
        StartCoroutine(ReloadGame(3f));
    }

    //the start of the level (tight spiral, not playable) starts at a higher timescale to rush through it and gradually reduces to 1f
    private IEnumerator AdjustTimeScale(float startDelay, float frequency) {
        yield return new WaitForSeconds(startDelay);
        while (Time.timeScale > 1f) {
            Time.timeScale = Time.timeScale - 0.1f;
            yield return new WaitForSeconds(frequency);
        }
        Time.timeScale = 1f;
    }

    private IEnumerator AddCorridorTiles(float startDelay, float frequency) {
        yield return new WaitForSeconds(startDelay);
        while (true) {
            AddTile(TileType.ceiling, CorridorHeight() / 2f);
            AddTile(TileType.floor, -CorridorHeight() / 2f);
            yield return new WaitForSeconds(frequency);
        }
    }

    private IEnumerator AddObstacleTiles(float startDelay, float frequency) {
        yield return new WaitForSeconds(startDelay);
        while (true) {
            audio.PlayOneShot(sfxObstacleSpawn);
            //generates two obstacles at a random height in the corridor with a fixed distance between them
            float r = Random.Range((-CorridorHeight() / 2f) + 3f, (CorridorHeight() / 2f)) - 3f;
            AddTile(TileType.obstacle, r - 3f);
            AddTile(TileType.obstacle, r + 3f);
            sunAnimator.SetTrigger("ObstacleFired");
            yield return new WaitForSeconds(frequency);
        }
    }

    private void AddTile(TileType type, float tileHeight) {
        //destination is the position the tile should end up in, but it's not spawned there for cool animation reasons
        Vector2 destination = currentLayout.GetCoord(Time.timeSinceLevelLoad, tileHeight);
        //floor and obstacle tiles spawn from the sun and go outwards towards destination
        float startOffset = -destination.magnitude + 0.1f;
        //ceiling tiles spawn from outside the screen and go inwards towards destination
        if (type == TileType.ceiling) startOffset = 50f;
        Vector2 pos = currentLayout.GetCoord(Time.timeSinceLevelLoad, tileHeight + startOffset);
        GameObject tile = Instantiate(Resources.Load(tilePrefabPaths[type]), pos, Quaternion.identity) as GameObject;
        tile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Time.timeSinceLevelLoad * Mathf.Rad2Deg));
        //tiles at the start of the spiral are scaled down, since the spiral is very tight
        tile.transform.localScale *= CorridorHeight() / maxCorridorHeight;
        tile.GetComponent<Tile>().destination = destination;
    }

    //how tall/high the corridor should be - starts off tiny to fit the tight start of the spiral, then keeps increasing until maxCorridorHeight
    private float CorridorHeight() {
        if (Time.timeSinceLevelLoad < maxCorridorHeight) {
            return Time.timeSinceLevelLoad;
        }
        return maxCorridorHeight;
    }

    private IEnumerator ReloadGame(float delay) {
        yield return new WaitForSeconds(delay);
        Application.LoadLevel(0);
    }

    private IEnumerator IncrementScore(float startDelay, float frequency) {
        yield return new WaitForSeconds(startDelay);
        while (!gameOver) {
            score += 1;
            scoreText.text = score.ToString();
            yield return new WaitForSeconds(frequency);
        }
    }

    private IEnumerator PlayOhNoSFX(float delay) {
        yield return new WaitForSeconds(delay);
        audio.PlayOneShot(sfxOhNo);
    }

    private IEnumerator EndIntroScreen() {
        yield return new WaitForSeconds(gameStartTime - 1f);
        //display GO!
        readyText.text = "GO!";
        audio.PlayOneShot(sfxGo);

        //1 second later, remove all the intro screen text
        yield return new WaitForSeconds(1f);
        Destroy(readyText);
        Destroy(authorText);
        Destroy(titleText);
        Destroy(instructionText);
        scoreText.enabled = true;
        bestScoreText.enabled = true;
    }

}
