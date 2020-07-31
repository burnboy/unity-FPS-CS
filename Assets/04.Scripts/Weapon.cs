using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public int bulletsPerMag;
    public int bulletsTotal;
    public int currentBullets;
    public float range;
    public float fireRate;
    public Vector3 aimPosition;
    private Vector3 originalPosition;
    public float accuracy;
    private float originalAccuracy;
    public float damage;

    private float fireTimer;
    public Transform shootPoint;
    private Animator anim;
    public ParticleSystem muzzleFlash;
    public Text bulletsText;

    //탄피
    public Transform bulletCasingPoint;


    private bool isReloading;
    private bool isAiming;

    //사운드
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;

    //프리팹
    public GameObject hitHolePrefab;
    public GameObject hitSparkPrefab;
    public GameObject BulletCasingPrefab;

    //반동
    public Transform camRecoil;
    public Vector3 recoilkickback;
    public float recoilAmount;
    private float originalRecoil;

    // Start is called before the first frame update
    void Start()
    {
        currentBullets = bulletsPerMag;
        anim = GetComponent<Animator>();
        bulletsText.text = currentBullets + "/" + bulletsTotal;
        originalPosition = transform.localPosition;
        originalAccuracy = accuracy;
        originalRecoil = recoilAmount;
    }

    // Update is called once per frame
    void Update()
    {
        //AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        //isReloading = info.IsName("Reload");

        Fire_Button();
        //if (Input.GetButton("Fire1"))
        //{
        //    if (currentBullets > 0)
        //    {
        //        Fire();
        //    }
        //}

        //if (fireTimer < fireRate)
        //{
        //    fireTimer += Time.deltaTime;
        //}
    }

    void Fire_Button()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        isReloading = info.IsName("Reload");

        if (Input.GetButton("Fire1"))
        {
            if (currentBullets > 0)
            {
                Fire();
            }
            else
            {
                DoReload();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            DoReload();

        }

        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
        AimDownSights();
        RecoilBack();
    }

    void Fire()
    {
        if (fireTimer < fireRate||isReloading)
        {
            return;
        }
        Debug.Log("Fired!");
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, shootPoint.transform.forward+Random.onUnitSphere*accuracy, out hit, range))
        {
            Debug.Log("Hit!");
            GameObject hitSpark = Instantiate(hitSparkPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            hitSpark.transform.SetParent(hit.transform);
            Destroy(hitSpark, 0.5f);
            GameObject hitHole = Instantiate(hitHolePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            hitHole.transform.SetParent(hit.transform);
            Destroy(hitHole, 5f);

            HealthManager healthManager = hit.transform.GetComponent<HealthManager>();
            if (healthManager)
            {
                healthManager.ApplyDamage(damage);
            }

            Rigidbody rigidbody = hit.transform.GetComponent<Rigidbody>();
            if (rigidbody)
            {

                rigidbody.AddForceAtPosition(transform.forward * 5f * damage, transform.position);
            }


        }
        currentBullets--;
        fireTimer = 0.0f;
        anim.CrossFadeInFixedTime("Fire", 0.01f);
        audioSource.PlayOneShot(shootSound);
        muzzleFlash.Play();
        bulletsText.text = currentBullets + "/" + bulletsTotal;
        Recoil();
        BulletEffect();
    }

    void DoReload()
    {
        if (!isReloading && currentBullets < bulletsPerMag && bulletsTotal > 0)
        {
            anim.CrossFadeInFixedTime("Reload", 0.01f);
            audioSource.PlayOneShot(reloadSound);
        }
    }

    public void Reload()
    {
        int bulletsToReload = bulletsPerMag - currentBullets;
        if (bulletsToReload > bulletsTotal)
        {
            bulletsToReload = bulletsTotal;
        }

        currentBullets += bulletsToReload;
        bulletsTotal -= bulletsToReload;
        bulletsText.text = currentBullets + "/" + bulletsTotal;
    }

    //정조준
    private void AimDownSights()
    {
        if (Input.GetButton("Fire2") && !isReloading)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, Time.deltaTime * 8f);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 40f, Time.deltaTime * 8f);
            isAiming = true;
            accuracy = originalAccuracy / 2f;
            recoilAmount = originalRecoil / 2f;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 5f);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 50f, Time.deltaTime * 8f);
            isAiming = false;
            accuracy = originalAccuracy;
            recoilAmount = originalRecoil;
        }

    }

    //반동
    private void Recoil()
    {
        Vector3 recoilVector = new Vector3(Random.Range(-recoilkickback.x, recoilkickback.x), recoilkickback.y, recoilkickback.z);
        Vector3 recoilCamVector = new Vector3(-recoilVector.y * 400f, recoilVector.x * 200f, 0);

        transform.localPosition = 
            Vector3.Lerp(transform.localPosition, transform.localPosition + recoilVector, recoilAmount / 2f);

        camRecoil.localRotation =
            Quaternion.Slerp(camRecoil.localRotation, Quaternion.Euler(camRecoil.localEulerAngles + recoilCamVector), recoilAmount);
        
    }

    private void RecoilBack()
    {
        camRecoil.localRotation = Quaternion.Slerp(camRecoil.localRotation, Quaternion.identity, Time.deltaTime * 2f);

    }

    private void BulletEffect()
    {
        Quaternion randomQuaternion = new Quaternion(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f), 1);
        GameObject casing = Instantiate(BulletCasingPrefab, bulletCasingPoint);
        casing.transform.localRotation = randomQuaternion;
        casing.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(Random.Range(50f, 100f), Random.Range(50f, 100f), Random.Range(-30f, 30f)));
        Destroy(casing, 1f);
    }
}
