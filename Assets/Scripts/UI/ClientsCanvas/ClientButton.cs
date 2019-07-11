using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ClientButton : MonoBehaviour
{
    [SerializeField]
    private Text ClientName = null;
    [SerializeField]
    private Text phoneNumber = null;
    [SerializeField]
    private Text clientEmail = null;
    [SerializeField]
    private Text clientAdress = null;
    [SerializeField]
    private Button clientButton;
    [SerializeField]
    private Button phoneButton;
    [SerializeField]
    private Button smsButton;
    [SerializeField]
    private Button mailButton;
    [SerializeField]
    private Button editButton;
    [SerializeField]
    private RectTransform clientBtn;
    [SerializeField]
    private Toggle clientToggle;
    [SerializeField]
    private RectTransform textSize;
    [SerializeField]
    private RectTransform detailSize;
    [SerializeField]
    private RectTransform triangleImage;
    [SerializeField]
    private GameObject footer;
    private float containerSize;
    private float initialContainerSize;
   
    public void Start()
    {

        //containerSize = textSize.sizeDelta.y;
        containerSize = textSize.rect.height;
        initialContainerSize = detailSize.sizeDelta.y;

    }
    public void Initialize(IClient client,Action<IClient> phoneCallBack, Action<IClient> smsCallback, Action<IClient> mailCallback, Action<IClient> editCallback)
    {
        ClientName.text = client.Name;
        phoneNumber.text = client.Number; 
        clientEmail.text = client.Email;
        clientAdress.text = client.Adress;
        phoneButton.onClick.AddListener(() => phoneCallBack(client));
        smsButton.onClick.AddListener(() => smsCallback(client));
        mailButton.onClick.AddListener(() => mailCallback(client));
        editButton.onClick.AddListener(() => editCallback(client));
    }

    public void AnimationPrefab()
    {
        if (clientToggle.isOn)
        {
            if (string.IsNullOrEmpty(phoneNumber.text) == false)
            {
                phoneNumber.gameObject.SetActive(true);
                RebuildUI();
                containerSize += initialContainerSize;
                
            }
            if (string.IsNullOrEmpty(clientEmail.text) == false)
            {
                clientEmail.gameObject.SetActive(true);
                RebuildUI();
                containerSize +=initialContainerSize;
               
            }
            if (string.IsNullOrEmpty(clientAdress.text) == false)
            {
                clientAdress.gameObject.SetActive(true);
                RebuildUI();
                containerSize += initialContainerSize;
                
            }

            footer.SetActive(true);
            RebuildUI();
            containerSize += initialContainerSize + 20;
           
            AnimateTriangle(0, 180);
            FadeIn();
        }
        else
        {
            containerSize =textSize.sizeDelta.y;
            Debug.Log(containerSize);
            phoneNumber.gameObject.SetActive(false);
            clientEmail.gameObject.SetActive(false);
            clientAdress.gameObject.SetActive(false);
            footer.SetActive(false);
            AnimateTriangle(180,0);
            FadeOut();
        }
    }
    
    public void AnimateTriangle(float from, float to)
    {
        float angle = Mathf.LerpAngle(from, to, Time.time/0.4f);
        triangleImage.transform.eulerAngles = new Vector3(0, 0, angle);
    }
    IEnumerator Move(RectTransform rt, Vector2 targetPos)
    {
        float step = 0;
        while (step < 1)
        {
            rt.sizeDelta = Vector2.Lerp(rt.sizeDelta, targetPos, step += Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(Move(clientBtn, new Vector2(1080, containerSize)));

    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(Move(clientBtn, new Vector2(900, containerSize)));
    }

    public bool SearchClients(string input)
    {
        bool ok = false;

        if (ClientName.text.ToLower().Trim().StartsWith(input.ToLower().Trim()) || phoneNumber.text.ToLower().Trim().Contains(input.ToLower().Trim()) || clientEmail.text.ToLower().Trim().StartsWith(input.ToLower().Trim()) || clientAdress.text.ToLower().Trim().StartsWith(input.ToLower().Trim()))
        {
            ok = true;
        }
        return ok;
    }

    private void RebuildUI()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(textSize);
        Canvas.ForceUpdateCanvases();
    }
}
