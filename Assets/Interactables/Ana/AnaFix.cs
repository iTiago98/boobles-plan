using UnityEngine;
using DG.Tweening;
using Booble.Interactables.Dialogues;

namespace Booble.Interactables.Ana
{
	public class AnaFix : MonoBehaviour
	{
		[SerializeField] private Transform _anaT;
		[SerializeField] private Transform _citrianoT;
		[SerializeField] private Vector3 _newCitrianoPos;
		[SerializeField] private Vector3 _vendingPos;
		[SerializeField] private Vector3 _coffePos;
		[SerializeField] private Vector3 _juicePos;
		[SerializeField] private float _walkSpeed;
		[SerializeField] private float _repairDuration;
		[SerializeField] private Animator _anim;
		[SerializeField] private Animator _citrianoAnim;
		[SerializeField] private Interactable _citrianoInteractable;
		[SerializeField] private Dialogue _newContinue;
		[SerializeField] private Collider2D _juiceCollider;

		public void FixMachine()
		{
			_citrianoInteractable.ChangeDialogue(_newContinue);
			_juiceCollider.enabled = true;

			Sequence seq = DOTween.Sequence();

			_citrianoAnim.SetTrigger("Walk");
			_citrianoT.rotation = Quaternion.Euler(0,180,0);
			_citrianoT.DOMove(_newCitrianoPos, 1).SetEase(Ease.Linear).OnComplete(() => _citrianoAnim.SetTrigger("Idle"));
			
			_anaT.rotation = Quaternion.Euler(0,180,0);
			_anim.SetTrigger("Walk");
			seq.Append(_anaT.DOMove(_coffePos, Mathf.Abs(_vendingPos.x - _coffePos.x)/_walkSpeed).SetEase(Ease.Linear).OnComplete(() => _anim.SetTrigger("Fix")));
			seq.AppendInterval(_repairDuration);
			seq.AppendCallback(() => _anim.SetTrigger("Walk"));
			seq.Append(_anaT.DOMove(_juicePos, Mathf.Abs(_coffePos.x - _juicePos.x)/_walkSpeed).SetEase(Ease.Linear).OnComplete(() => _anim.SetTrigger("Fix")));
			seq.AppendInterval(_repairDuration);
			seq.AppendCallback(() => _anaT.rotation = Quaternion.identity);
			seq.AppendCallback(() => _anim.SetTrigger("Walk"));
			seq.Append(_anaT.DOMove(_vendingPos, Mathf.Abs(_juicePos.x - _vendingPos.x)/_walkSpeed).SetEase(Ease.Linear).OnComplete(() => _anim.SetTrigger("Fix")));
			seq.OnComplete(() => Interactable.EndInteraction());
		}
	}
}