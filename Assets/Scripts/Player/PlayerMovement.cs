using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 6.0f;

	private Vector3 movement; //movimentação
	private Animator anim; //animações
	private Rigidbody playerRigidbody; //Rigidbody
	private int floorMask; //máscara de colisão que será levada em consideração para a câmera acompanhar
	private float camRayLength = 100.0f; //distância máxima do raio invisível criado pela câmera para acompanhar objetos

	//executado ao iniciar, independente se o script está ativado ou não
	void Awake()
	{
		floorMask = LayerMask.GetMask ("Floor"); //pega todas as máscaras que estão no layer "Floor"
		anim = GetComponent<Animator> (); //pega componente de animações
		playerRigidbody = GetComponent<Rigidbody> (); //pega componente de physics
	}

	//executa a cada update em physics
	void FixedUpdate()
	{
		//pega inputs de movimentações
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

		Move (h, v); //movimenta o player
		Turning (); //trata a direção que o player estará
		Animating (h, v); //anima o player
	}

	//movimenta o jogador de acordo com os inputs passados
	void Move(float h, float v)
	{
		//define uma movimentação de acordo com o passado
		movement.Set (h, 0.0f, v);

		//normaliza o movimento do vetor e deixa proporcional a velocidade por segundos
		movement = movement.normalized * speed * Time.deltaTime;

		//move o personagem: posição atual + nova posição de movimento
		playerRigidbody.MovePosition (transform.position + movement);
	}

	//faz o personagem ficar na direção em que o mouse está, se este estiver em cima do chão
	void Turning()
	{
		//faz um raio invisível da câmera até o mouse
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

		//responsável por guardar dados do raio que encostou em algo
		RaycastHit floorHit;

		//se o raio está encostando na máscara dos objetos que estão na camada "Floor"
		if (Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
		{
			//cria um vetor da diferença da posição do personagem com onde o mouse encostou
			Vector3 playerToMouse = floorHit.point - transform.position;
			playerToMouse.y = 0f; //certifica-se que não virará verticalmente

			//roda o jogador
			Quaternion newRotation = Quaternion.LookRotation (playerToMouse);
			playerRigidbody.MoveRotation(newRotation);
		}
	}

	//responsável por definir as animações
	void Animating(float h, float v)
	{
		//define animação de andar
		bool walking = h != 0 || v != 0;
		anim.SetBool ("IsWalking", walking);
	}
}
