using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

    //where this tile needs to be (which is not where it is spawned, for cool animation reasons)
    public Vector2 destination = Vector2.zero;

    //how long before this tile deletes itself
    private float destructionDelay = 5f;

    private void Awake() {
        StartCoroutine(DestroySelf(destructionDelay));
    }

    private void FixedUpdate() {
        //this moves the tile gradually closer to where it needs to be, which makes for a neat spawning animation pattern
        Vector2 nextMove = destination - (Vector2)transform.position;
        nextMove *= 0.1f;
        transform.position = new Vector2(nextMove.x + transform.position.x, nextMove.y + transform.position.y);
    }

    //with more development time, instead of deleting and spawning new tiles, I'd probably try having a constant pool of tiles and just move them around appropriately
    private IEnumerator DestroySelf(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
