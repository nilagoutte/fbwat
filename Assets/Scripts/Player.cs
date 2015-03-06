using UnityEngine;

public class Player : MonoBehaviour {

    public GameObject deathParticleSystem;
    public Game game;
    public ParticleSystem jumpParticleSystem;

    //remember the player's current height in the twist coordinate system - height 0 is the middle of the corridor
    private float height = 0f;
    //remember the player's current y velocity in the twist coordinate system
    private float verticalVelocity = 0f;
    //gravity pulls towards the sun (0, 0, 0), not the bottom of the screen
    private float gravity = -0.01f;
    private float jumpVelocity = 0.2f;
    private bool jump = false;

    //player input in Update to avoid missed frames
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
        if (Time.timeSinceLevelLoad < game.gameStartTime) return;
        if (Input.GetKeyDown(KeyCode.Space)) {
            jump = true;
        }
    }

    private void FixedUpdate() {
        if (Time.timeSinceLevelLoad < game.gameStartTime - 3f) return;

        if (Time.timeSinceLevelLoad > game.gameStartTime) {
            //update the internal model
            //if we caught a jump input, jump
            if (jump) {
                game.audio.PlayOneShot(game.sfxJump);
                verticalVelocity = jumpVelocity;
                jumpParticleSystem.Play();
                jump = false;
            }
            verticalVelocity += gravity;
            height += verticalVelocity;
        }

        float playerTime = Time.timeSinceLevelLoad - game.levelConstructionDelay;

        //update the view (apply the model to the actual unity coordinates)
        Vector2 newPosition = game.currentLayout.GetCoord(playerTime, height);
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, playerTime * Mathf.Rad2Deg));
    }

    //runs when the player collides with a tile (game over)
    private void OnTriggerEnter2D(Collider2D deathTile) {
        //disable the player, hide the sprite since it's "dead" and stop any further collisions
        enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        collider2D.enabled = false;
        //release blood particles
        Instantiate(deathParticleSystem, transform.position, Quaternion.identity);
        //run game over code
        game.GameOver(deathTile);
    }

}