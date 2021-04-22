using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class NarutoKidCommands : MonoBehaviour
{
	private PlayerController playerController; //Para trabalhar com o comportamento do Player, previamente codificado.
	private bool isJumping; //FLAG de pulo para controle neste classe também.

	[SerializeField] private Animator m_Animator; //Componente de Animação

	private void Awake()
	{
		this.playerController = GetComponent<PlayerController>(); //Carregar o Componente PlayerController.
	}


	// Update is called once per frame
	private void Update()
	{
		if (!isJumping)
		{ //Identificando o botão de PULAR, SOMENTE se já não estiver pulando.
			this.isJumping = Input.GetButtonDown("Jump"); //Com isso, atualiza a variável de controle local.
		}
	}

	private void FixedUpdate()
	{
		float isMoving = Input.GetAxis("Horizontal"); //Identifica se o botão de andar horizontalmente foi acionado

		playerController.Move(isMoving, isJumping, false); //Ativa a função de isMoving, mesmo que o isMoving seja 0.
		isJumping = false; //E volta a variável de controle de PULO para falso.
	}

	public void OnGroundCollision(bool onTheGround)
    {
		m_Animator.SetBool("onTheGround", onTheGround);
	}

	public void OnMove(float speed)
	{
		m_Animator.SetFloat("speed", speed);
	}
}
