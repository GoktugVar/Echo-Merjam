using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI Play;
    public TextMeshProUGUI textMesh;
    [TextArea]
    public string metin;
    public Image Panel;
    async void Start()
    {
        await Task.Delay(4000);
        Play.DOFade(1, 3);   
    }

    public async void ChangeScene()
    {
        // Play objesinin alfa de�erini 1 saniyede s�f�ra d���r
        await Play.DOFade(0, 1).AsyncWaitForCompletion();

        // Play objesini devre d��� b�rak ve textMesh'i etkinle�tir
        Play.gameObject.SetActive(false);
        textMesh.gameObject.SetActive(true);

        // textMesh'in alfa de�erini 1 saniyede 1'e y�kselt
        await textMesh.DOFade(1, 1).AsyncWaitForCompletion();

        // Metni h�zl�ca yazd�r
        textMesh.text = ""; // Metni bo�alt
        DOTween.To(() => textMesh.text.Length, x => textMesh.text = metin.Substring(0, x), metin.Length, metin.Length * 0.1f)
            .SetEase(Ease.Linear)
            .SetDelay(.1f)
            .OnComplete(async () =>
            {
                await Task.Delay(4000); // 4 saniye bekle
                                        // Panel objesinin alfa de�erini 1 saniyede 1'e y�kselt
                await Panel.DOFade(1, 1).AsyncWaitForCompletion();

                // Bir sonraki sahneye ge�
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
                SceneManager.LoadSceneAsync(nextSceneIndex);
            });
    }


    public void Quit()
    {
        Application.Quit();
    }
}
