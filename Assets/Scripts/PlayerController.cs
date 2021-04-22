using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	[SerializeField] bool m_CanMoveInAir = true; //Indicar se pode haver isMoving para os lados durante o pulo.
	[SerializeField] private bool m_CanMove = true;

	[Header("Physics")]
	[Space]
	[SerializeField] float m_JumpForce = 400f; //Valor da força aplicada para pulo.
	[SerializeField] float m_JumpForceInRunning = 500f; //Valor da força aplicada para pulo.
	[SerializeField] float m_SpeedNormal = 6f; // Valor de velocidade normal (Andando)
	[SerializeField] float m_SpeedFast = 12f; //Valor de velocidade Rapida (Correndo)

	[Header("References")]
	[Space]
	[SerializeField] private LayerMask m_GroundLayers; //Listar Layer que indicam quais objetos são chão 
	[SerializeField] private Transform m_GroundCollider; // Objeto que marca o pé do player.
	[SerializeField] private Animator m_Animator; //Componente de Animação
	private Rigidbody2D m_Rigidbody2D; //Componente de Rigidbody

	private bool m_IsDead = false;//FLAG indicando se o player está morto
	private const float radioReached = .2f; //Valor da distancia a ser detectada proximo do "Chão"
	private bool m_OnTheGround; //FLAG que indica se o Player está tocando Chão.
	private bool m_Flipped = false;//FLAG indicando direção do Player (Direita ou Esquerda)

	[Header("Events")]
	[Space]
	public UnityEvent<bool> OnGroundCollision;
	public UnityEvent<float> OnMove;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnGroundCollision == null)
			OnGroundCollision = new UnityEvent<bool>();

		if (OnMove == null)
			OnMove = new UnityEvent<float>();
	}
	private void FixedUpdate()
	{

		this.m_OnTheGround = false;

		/*
		 * Verificando os objetos que estão entrando em colisão com o objeto marcado como Pé (m_GroundCollider)
		 * A uma certa distância "radioReached"
		 * Indicados pelos seguintes Layers (m_GroundLayers)
		*/
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCollider.position, radioReached, m_GroundLayers);
		// E se qualquer um desses objetos for diferente do Player (que pode acontecer), significa que o PLAYER está no chão
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
				this.m_OnTheGround = true;
		}

		OnGroundCollision.Invoke(m_OnTheGround);
	}

	//SERVE PARA INVERTER O LADO DO SPRITE EM X.
	private void Flip()
	{

		this.m_Flipped = !this.m_Flipped;

		Vector3 currentScale = this.transform.localScale;
		currentScale.x *= -1;
		transform.localScale = currentScale;

	}

	public void Move(float isMoving, bool isJumping, bool isRunning = false)
	{

		//Verifica se o Player está no chao ou se pode mover no ar para poder efetuar isMoving.
		if ((m_OnTheGround || m_CanMoveInAir) && m_CanMove)
		{

			OnMove.Invoke(Mathf.Abs(isMoving));

			//Identifica qual das velocidades será usada, a de Correr ou Caminhar.
			var speed = (isRunning) ? m_SpeedFast : m_SpeedNormal;

			//Aplica a força velocidade nova em X e mantém a velocidade de Y, pois pode ser que o mesmo esteja no ar.
			this.m_Rigidbody2D.velocity = new Vector2(isMoving * speed, m_Rigidbody2D.velocity.y);

			//Valida a direção que segue e vira o Sprite se for necessario ----
			if (isMoving > 0 && m_Flipped)
			{
				this.Flip();
			}

			if (isMoving < 0 && !m_Flipped)
			{
				this.Flip();
			}
			//------------------------------------------------------------------

			/*
			 * Caso o isMoving peça para pular, deve ser verificado se o mesmo está no chão
			 * E caso esteja tudo certo, atualiza...
			*/
			if (m_OnTheGround && isJumping && m_Animator.GetBool("onTheGround"))
			{
				m_OnTheGround = false; //Atualiza a FLAG de estar no Chão
				OnGroundCollision.Invoke(false);

				var force = (isRunning) ? m_JumpForceInRunning : m_JumpForce;

				m_Rigidbody2D.AddForce(new Vector2(0f, force)); //Aplica força no eixo Y, simulando o PULO.
			}

		}

	}

	public void Morrer()
	{

		if (!this.m_IsDead)
		{
			this.m_CanMove = false;
			this.m_IsDead = true;

			this.m_Animator.SetBool("Morto", m_IsDead);
			this.m_Animator.SetTrigger("Morrer");
		}
	}
}
