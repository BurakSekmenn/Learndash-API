LEARNDASH APİ KULLANIMI
* APİ İSTEK ATMADAN ÖNCE wp-content/plugins/sfwd-lms içerisindeki 2 tane dosyayı bu aşağıdaki şekilde düzenlenmeli

1)	learndash-scaler-constants.php içerisinde rest api aktifleştirilmesi  gerekiyor.
       - LEARNDASH_REST_API_ENABLED true yapılmalı
2) 	learndash-features-constants.php içerisinde
      -LEARNDASH_ENABLE_IN_PROGRESS_FEATURES  değeri false yapılmalı
      -LEARNDASH_ENABLE_FEATURE_BREEZY_TEMPLATE değeri false yapılmalı

