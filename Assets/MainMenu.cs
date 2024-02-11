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
        // Play objesinin alfa deðerini 1 saniyede sýfýra düþür
        await Play.DOFade(0, 1).AsyncWaitForCompletion();

        // Play objesini devre dýþý býrak ve textMesh'i etkinleþtir
        Play.gameObject.SetActive(false);
        textMesh.gameObject.SetActive(true);

        // textMesh'in alfa deðerini 1 saniyede 1'e yükselt
        await textMesh.DOFade(1, 1).AsyncWaitForCompletion();

        // Metni hýzlýca yazdýr
        textMesh.text = ""; // Metni boþalt
        DOTween.To(() => textMesh.text.Length, x => textMesh.text = metin.Substring(0, x), metin.Length, metin.Length * 0.1f)
            .SetEase(Ease.Linear)
            .SetDelay(.1f)
            .OnComplete(async () =>
            {
                await Task.Delay(4000); // 4 saniye bekle
                                        // Panel objesinin alfa deðerini 1 saniyede 1'e yükselt
                await Panel.DOFade(1, 1).AsyncWaitForCompletion();

                // Bir sonraki sahneye geç
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
