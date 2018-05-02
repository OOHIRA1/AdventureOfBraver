using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour {

    Rigidbody rb;
    GameObject child;
    Animation ch_anim;
	Transform tra_me;

    GameObject target;

    [SerializeField]
    float accelation = 3;
	[SerializeField]
	float VELOCITY_MAX = 10;
	[SerializeField]
	float NEAR_LENGTH = 3;

    // 確認用　の　変数。
    //[SerializeField];

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>( );
        child = transform.Find( "spider_myOldOne" ).gameObject;
        ch_anim = child.GetComponent<Animation>( );

		tra_me = GetComponent<Transform>( );
	}
	
	// Update is called once per frame
	void Update () {

		rb.angularVelocity = new Vector3 ( 0,0,0 );
        if ( target != null ) {
            Vector3 length = target.transform.position - tra_me.position;
			tra_me.rotation = Quaternion.LookRotation( length );

			if ( length.magnitude > NEAR_LENGTH ) {
				Vector3 vec = length.normalized;
                vec.Scale(new Vector3(accelation, 0, accelation));
                rb.AddForce( vec );

				if (rb.velocity.magnitude > VELOCITY_MAX) {
					rb.velocity = Vector3.Scale ( rb.velocity.normalized, new Vector3 (VELOCITY_MAX,VELOCITY_MAX,VELOCITY_MAX ) );;
				}
            }
               
        } else {
           
        }

        if ( rb.velocity.magnitude > 0 ) {
            ch_anim.Play( "walk" );
        } else {
            ch_anim.Play("idle");
        }



        
	}


	void OnTriggerStay( Collider other ) {
		if (other.gameObject.tag == "Player") {

			target = other.gameObject;

		}
	}

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {

            target = null;

        }
    }

}


