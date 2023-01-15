using Booble.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] private float _scrollSpeed;
    [SerializeField] private CanvasScaler _cv;
    
    private RectTransform _rt;
    private bool _finished;
    
    private void Awake()
    {
        #region SetText
        GetComponent<TMP_Text>().text = "Booble's Plan" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Productor" + "\n" +
                                        "" + "\n" +
                                        "Santiago Lete Martín" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Directores de programación" + "\n" +
                                        "" + "\n" +
                                        "Santiago Lete Martín" + "\n" +
                                        "Lluis Miralles García" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Directora de Arte" + "\n" +
                                        "" + "\n" +
                                        "Ana María Valencia Pareja" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Director Narrativo" + "\n" +
                                        "" + "\n" +
                                        "Ignacio Batlles Gómez" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Director de Sonido" + "\n" +
                                        "" + "\n" +
                                        "Albert Martín-Lorente Romero" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Diseño de Niveles" + "\n" +
                                        "" + "\n" +
                                        "Ignacio Batlles Gómez" + "\n" +
                                        "Santiago Lete Martín" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Diseño de jugabilidad de entrevistas" + "\n" +
                                        "" + "\n" +
                                        "Santiago Lete Martín" + "\n" +
                                        "Lluís Miralles García" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Diseño de entrevistas" + "\n" +
                                        "" + "\n" +
                                        "Ignacio Batlles Gómez" + "\n" +
                                        "Santiago Lete Martín" + "\n" +
                                        "Lluis Miralles García" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Diseño de jugabilidad de exploración" + "\n" +
                                        "" + "\n" +
                                        "Santiago Lete Martín" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Diseño de GUI" + "\n" +
                                        "" + "\n" +
                                        "Lluís Miralles García" + "\n" +
                                        "Ana María Valencia Pareja" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Diseño de HUD" + "\n" +
                                        "" + "\n" +
                                        "Ana María Valencia Pareja" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Diseño artístico de cartas" + "\n" +
                                        "" + "\n" +
                                        "Rafael Jesús Hidalgo Clérico" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Diseño artístico de escenarios" + "\n" +
                                        "" + "\n" +
                                        "Ana María Valencia Pareja" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Diseño artístico de personajes" + "\n" +
                                        "" + "\n" +
                                        "Ana María Valencia Pareja" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Diseño artístico de animación de personajes" + "\n" +
                                        "" + "\n" +
                                        "Rafael Jesús Hidalgo Clérico" + "\n" +
                                        "Ana María Valencia Pareja" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Diseño Narrativo" + "\n" +
                                        "" + "\n" +
                                        "Ignacio Batlles Gómez" + "\n" +
                                        "Santiago Lete Martín" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Diseño de sonido" + "\n" +
                                        "" + "\n" +
                                        "Albert Martín-Lorente Romero" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Programación de exploración" + "\n" +
                                        "" + "\n" +
                                        "Santiago Lete Martín" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Programación de entrevistas" + "\n" +
                                        "" + "\n" +
                                        "Lluis Miralles García" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Programación de interfaz" + "\n" +
                                        "" + "\n" +
                                        "Santiago Lete Martín" + "\n" +
                                        "Lluis Miralles" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Programación de audio" + "\n" +
                                        "" + "\n" +
                                        "Lluis Miralles" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Arte de GUI" + "\n" +
                                        "" + "\n" +
                                        "Ana María Valencia Pareja" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Arte de HUD" + "\n" +
                                        "" + "\n" +
                                        "Ana María Valencia Pareja" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Arte de cartas" + "\n" +
                                        "" + "\n" +
                                        "Rafael Jesús Hidalgo Clérico" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "VFX" + "\n" +
                                        "" + "\n" +
                                        "Rafael Jesús Hidalgo Clérico" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Arte de escenarios" + "\n" +
                                        "" + "\n" +
                                        "Rafael Jesús Hidalgo Clérico" + "\n" +
                                        "Ana María Valencia Pareja" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Arte de personajes" + "\n" +
                                        "" + "\n" +
                                        "Rafael Jesús Hidalgo Clérico" + "\n" +
                                        "Ana María Valencia Pareja" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Arte de animaciones" + "\n" +
                                        "" + "\n" +
                                        "Rafael Jesús Hidalgo Clérico" + "\n" +
                                        "Ana María Valencia Pareja" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Banda sonora original" + "\n" +
                                        "" + "\n" +
                                        "Albert Martín-Lorente Romero" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Chacheador máximo" + "\n" +
                                        "" + "\n" +
                                        "Ignacio Batlles Gómez" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Censuradora oficial" + "\n" +
                                        "" + "\n" +
                                        "Ana María Valencia Pareja" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Mayor censurado" + "\n" +
                                        "" + "\n" +
                                        "Ignacio Batlles Gómez" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Los que le dan cuerda al Chacho para que haga historias turbias que Ana tenga que censurar" + "\n" +
                                        "" + "\n" +
                                        "Santiago Lete Martín" + "\n" +
                                        "Lluís Miralles García" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Guapito de cara" + "\n" +
                                        "" + "\n" +
                                        "Rafael Jesús Hidalgo Clérico" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "" + "\n" +
                                        "Agradecimientos especiales" + "\n" +
                                        "" + "\n" +
                                        "Fulgencio Fornieles" + "\n" +
                                        "Arturo Nerfández" + "\n" +
                                        "Ramónica Hall" + "\n" +
                                        "" + "\n" +
                                        "Y por supuesto a" + "\n" +
                                        "" + "\n" +
                                        "Rosa Melano";
        #endregion

        _rt = (RectTransform)transform;
    }

    private void FixedUpdate()
    {
        if(_finished)
            return;
        
        _rt.anchoredPosition += Vector2.up * _scrollSpeed * Time.deltaTime;

        if (_rt.anchoredPosition.y < _cv.referenceResolution.y + _rt.sizeDelta.y)
            return;

        _finished = true;
        SceneLoader.Instance.LoadHomeEnding();
    }
    
}
