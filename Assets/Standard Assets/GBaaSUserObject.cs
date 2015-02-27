using System;

// **************************
// GBaaS 를 사용하기 위해 설정해야 하는 사용자 정보모음입니다.
// SDK를 업데이트 하여 받으실 경우에는 작성하신 이 파일의 이름을 변경하여
// 설정이 삭제되지 않도록 보관하시기를 권하여 드립니다.
// 새로 받은 파일에 기존 내용으로 수정하셔서 업데이트 하시길 바랍니다.
// **************************
public class GBaaSUserObject {
	// GBaaS API_ENDPOINT 값은
	// dev.gbaas.io 사이트에 가입 후 App을 생성하시면 확인 하실 수 있습니다.
	// 서비스 사이트의 개발자 가이드를 참고하십시오.
	// 아래에 입력되어 있는 값은 샘플 프로젝트 앱의 API_ENDPOINT 값입니다.
	// 직접 생성하신 App 의 정보로 변경하셔야 합니다.
	public static string API_ENDPOINT = "https://api.gbaas.io/33e8b61a-3340-11e4-ab01-b99509431e86/ecf9ff50-bd8e-11e4-88dd-950cc9400d23/"; //GBaaS Test Project
	public static string GOOGLE_PROJECT_NUM_FOR_GCM = "941440455383";
	
	// Xiaomi 연동 및 결제를 위한 상품 코드
	// Xiaomi 플랫폼 연동을 위한 경우 사전에 협의 부탁드립니다.
	// biz@apexplatform.net
	// Xiaomi 플랫폼은 APPID, APPKEY 가 발급된 후 사용가능하므로
	// 신청 후 일정 기간이 소요될 수 있습니다.
	public static string LEAN_APPID = "";
	public static string LEAN_APPKEY = "";

	public static string XIAOMI_APPID = "";
	public static string XIAOMI_APPKEY = "";
	public static string XIAOMI_PAY_CODE1 = "";
}
