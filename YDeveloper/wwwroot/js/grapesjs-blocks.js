
// ==========================================
// 🔥 ULTIMATE BLOK KÜTÜPHANESİ - 100+ BLOK 🔥
// ==========================================

function loadCustomBlocks(editor) {
    const bm = editor.BlockManager;

    // --- İKONLAR (SVG) ---
    const icons = {
        layout: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><rect x="3" y="3" width="18" height="18" rx="2"/><path d="M3 9h18"/><path d="M9 21V9"/></svg>',
        hero: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><rect x="3" y="3" width="18" height="18" rx="2"/><path d="M3 15h18"/><path d="M12 8l3 3-3 3-3-3 3-3z"/></svg>',
        feature: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/><rect x="14" y="14" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/></svg>',
        content: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><line x1="21" y1="10" x2="3" y2="10"/><line x1="21" y1="6" x2="3" y2="6"/><line x1="21" y1="14" x2="3" y2="14"/><line x1="21" y1="18" x2="3" y2="18"/></svg>',
        cta: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><rect x="3" y="5" width="18" height="14" rx="2"/><path d="M12 12h.01"/></svg>',
        team: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><circle cx="12" cy="7" r="4"/><path d="M5.5 21v-2a4 4 0 0 1 4-4h5a4 4 0 0 1 4 4v2"/></svg>',
        stats: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M18 20V10"/><path d="M12 20V4"/><path d="M6 20v-6"/></svg>',
        pricing: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M12 1v22"/><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/></svg>',
        faq: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><circle cx="12" cy="12" r="10"/><path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"/><path d="M12 17h.01"/></svg>',
        form: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><rect x="3" y="5" width="18" height="14" rx="2"/><path d="M12 12h.01"/></svg>',
        footer: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><rect x="3" y="15" width="18" height="6" rx="2"/></svg>',
        gallery: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><rect x="3" y="3" width="18" height="18" rx="2"/><circle cx="8.5" cy="8.5" r="1.5"/><polyline points="21 15 16 10 5 21"/></svg>',
    };

    // --- KATEGORİ TANIMLARI ---
    const cats = {
        layout: { id: 'layout', label: '🏗️ YAPI BLOKLARI', open: true },
        hero: { id: 'hero', label: '✨ HERO ALANLARI', open: false },
        feature: { id: 'feature', label: '🌟 ÖZELLİKLER', open: false },
        content: { id: 'content', label: '📝 İÇERİK', open: false },
        cta: { id: 'cta', label: '📣 ÇAĞRI (CTA)', open: false },
        team: { id: 'team', label: '👥 EKİP', open: false },
        stats: { id: 'stats', label: '📊 İSTATİSTİKLER', open: false },
        pricing: { id: 'pricing', label: '💰 FİYATLANDIRMA', open: false },
        testimonial: { id: 'testimonial', label: '💬 YORUMLAR', open: false },
        faq: { id: 'faq', label: '❓ S.S.S.', open: false },
        gallery: { id: 'gallery', label: '📷 GALERİ', open: false },
        form: { id: 'form', label: '📬 FORMLAR', open: false },
        footer: { id: 'footer', label: '🔻 ALT BİLGİ', open: false },
    };

    // Helper: Blok Ekleme
    const addBlock = (id, label, cat, icon, content) => {
        bm.add(id, {
            label: label,
            category: cat,
            media: icon,
            content: content
        });
    };

    // ==========================================
    // 1. YAPI BLOKLARI (LAYOUT)
    // ==========================================
    addBlock('sect-empty', 'Boş Bölüm', cats.layout, icons.layout, `<section style="padding: 70px 20px; min-height: 200px; background-color: #ffffff;"></section>`);
    addBlock('cont-fixed', 'Konteyner (Sabit)', cats.layout, icons.layout, `<div style="max-width: 1200px; margin: 0 auto; padding: 15px;"></div>`);
    addBlock('grid-2', '2 Kolon', cats.layout, icons.layout, `<div style="display: grid; grid-template-columns: 1fr 1fr; gap: 30px;"><div>Kolon 1</div><div>Kolon 2</div></div>`);
    addBlock('grid-3', '3 Kolon', cats.layout, icons.layout, `<div style="display: grid; grid-template-columns: 1fr 1fr 1fr; gap: 30px;"><div>Kolon 1</div><div>Kolon 2</div><div>Kolon 3</div></div>`);
    addBlock('grid-4', '4 Kolon', cats.layout, icons.layout, `<div style="display: grid; grid-template-columns: repeat(4, 1fr); gap: 20px;"><div>1</div><div>2</div><div>3</div><div>4</div></div>`);
    addBlock('grid-sidebar-right', 'Sağ Sidebar', cats.layout, icons.layout, `<div style="display: grid; grid-template-columns: 2fr 1fr; gap: 30px;"><div>Ana İçerik</div><aside>Yan Menü</aside></div>`);
    addBlock('grid-sidebar-left', 'Sol Sidebar', cats.layout, icons.layout, `<div style="display: grid; grid-template-columns: 1fr 2fr; gap: 30px;"><aside>Yan Menü</aside><div>Ana İçerik</div></div>`);

    // ==========================================
    // 2. HERO BLOKLARI (10+)
    // ==========================================
    addBlock('hero-gradient', 'Hero: Gradyan', cats.hero, icons.hero, `
        <section style="padding: 120px 20px; background: linear-gradient(135deg, #6366f1, #a855f7, #ec4899); color: white; text-align: center;">
            <h1 style="font-size: 3.5rem; margin-bottom: 20px; font-weight: 800; line-height: 1.1;">Geleceği İnşa Et</h1>
            <p style="font-size: 1.25rem; opacity: 0.9; max-width: 600px; margin: 0 auto 30px;">Modern web teknolojileri ile işinizi dijitale taşıyın.</p>
            <a href="#" style="background: white; color: #4f46e5; padding: 15px 40px; border-radius: 50px; text-decoration: none; font-weight: bold; box-shadow: 0 10px 25px rgba(0,0,0,0.1);">Hemen Başla</a>
        </section>
    `);

    addBlock('hero-image-right', 'Hero: Resim Sağda', cats.hero, icons.hero, `
        <section style="padding: 80px 20px; max-width: 1200px; margin: 0 auto; display: flex; align-items: center; gap: 50px; flex-wrap: wrap;">
            <div style="flex: 1; min-width: 300px;">
                <h1 style="font-size: 3rem; color: #1e293b; margin-bottom: 20px; line-height: 1.2;">Profesyonel Çözümler</h1>
                <p style="color: #64748b; font-size: 1.1rem; margin-bottom: 30px;">İhtiyacınız olan tüm araçlar tek bir platformda.</p>
                <button style="background: #2563eb; color: white; padding: 12px 30px; border: none; border-radius: 6px; font-size: 1rem;">Detaylı Bilgi</button>
            </div>
            <div style="flex: 1; min-width: 300px;">
                <img src="https://images.unsplash.com/photo-1498050108023-c5249f4df085?auto=format&fit=crop&w=800&q=80" style="width: 100%; border-radius: 20px; box-shadow: 0 20px 40px rgba(0,0,0,0.1);">
            </div>
        </section>
    `);

    addBlock('hero-video', 'Hero: Video Arkaplan', cats.hero, icons.hero, `
        <section style="padding: 150px 20px; position: relative; color: white; text-align: center; overflow: hidden; background: #000;">
            <div style="position: absolute; top:0; left:0; width:100%; height:100%; opacity: 0.5; background: url('https://images.unsplash.com/photo-1550751827-4bd374c3f58b?auto=format&fit=crop&w=1600&q=80') center/cover;"></div>
            <div style="position: relative; z-index: 2;">
                <h1 style="font-size: 4rem; text-shadow: 0 4px 8px rgba(0,0,0,0.3);">Sinematik Deneyim</h1>
                <p style="font-size: 1.5rem; text-shadow: 0 2px 4px rgba(0,0,0,0.3);">Markanızı hareketlendirin.</p>
            </div>
        </section>
    `);

    addBlock('hero-minimal', 'Hero: Minimal', cats.hero, icons.hero, `
        <section style="padding: 100px 20px; text-align: center; max-width: 800px; margin: 0 auto;">
            <h1 style="color: #111; font-size: 3.5rem; line-height: 1.2; letter-spacing: -1px;">Daha Az, Daha Çok.</h1>
            <p style="color: #666; font-size: 1.2rem; margin-top: 20px;">Sadelik en yüksek gelişmişlik düzeyidir.</p>
        </section>
    `);

    addBlock('hero-form', 'Hero: Kayıt Formlu', cats.hero, icons.hero, `
        <section style="padding: 80px 20px; background: #f8fafc;">
            <div style="max-width: 1100px; margin: 0 auto; display: flex; align-items: center; gap: 40px; flex-wrap: wrap;">
                <div style="flex: 1;">
                    <h1 style="font-size: 2.8rem; color: #0f172a; margin-bottom: 15px;">Bültenimize Katılın</h1>
                    <p style="color: #475569; font-size: 1.1rem;">En son haberleri ve güncellemeleri ilk siz öğrenin.</p>
                </div>
                <div style="flex: 1; min-width: 320px; background: white; padding: 40px; border-radius: 16px; box-shadow: 0 10px 30px rgba(0,0,0,0.05);">
                    <h3 style="margin-bottom: 20px;">Hemen Kaydol</h3>
                    <input type="email" placeholder="E-posta adresiniz" style="width: 100%; padding: 12px; margin-bottom: 15px; border: 1px solid #e2e8f0; border-radius: 8px;">
                    <button style="width: 100%; padding: 12px; background: #2563eb; color: white; border: none; border-radius: 8px; font-weight: 600;">Abone Ol</button>
                </div>
            </div>
        </section>
    `);

    addBlock('hero-dark-modern', 'Hero: Modern Koyu', cats.hero, icons.hero, `
        <section style="padding: 120px 20px; background: #0f172a; color: white; overflow: hidden; position: relative;">
            <div style="position: absolute; top: -50px; right: -50px; width: 300px; height: 300px; background: #3b82f6; filter: blur(100px); opacity: 0.2;"></div>
            <div style="position: relative; max-width: 1200px; margin: 0 auto; text-align: left;">
                <span style="color: #3b82f6; font-weight: 600; text-transform: uppercase; letter-spacing: 1px;">YENİ SÜRÜM 2.0</span>
                <h1 style="font-size: 4rem; margin: 20px 0; font-weight: 800; line-height: 1;">Limitleri Zorla.</h1>
                <p style="color: #94a3b8; font-size: 1.3rem; max-width: 600px; margin-bottom: 40px;">Teknoloji dünyasında sinırları kaldırın ve potansiyelinizi keşfedin.</p>
                <div style="display: flex; gap: 15px;">
                    <a href="#" style="background: #3b82f6; color: white; padding: 15px 35px; border-radius: 8px; text-decoration: none;">İncele</a>
                    <a href="#" style="border: 1px solid #334155; color: white; padding: 15px 35px; border-radius: 8px; text-decoration: none;">GitHub</a>
                </div>
            </div>
        </section>
    `);

    addBlock('hero-app', 'Hero: Uygulama Tanıtım', cats.hero, icons.hero, `
        <section style="padding: 100px 20px; text-align: center; background: #fff;">
            <h1 style="font-size: 3rem; color: #111;">Her Yerden Yönetin</h1>
            <p style="color: #666; margin-bottom: 40px;">Mobil uygulamamız ile işiniz cebinizde.</p>
            <div style="display: flex; justify-content: center; gap: 20px; margin-bottom: 50px;">
                <button style="background: #000; color: white; padding: 10px 25px; border-radius: 8px; border:none;"><i class="fab fa-apple"></i> App Store</button>
                <button style="background: #000; color: white; padding: 10px 25px; border-radius: 8px; border:none;"><i class="fab fa-google-play"></i> Play Store</button>
            </div>
            <img src="https://via.placeholder.com/800x400" style="width: 80%; max-width: 800px; border-radius: 20px; box-shadow: 0 20px 60px rgba(0,0,0,0.1);">
        </section>
    `);

    // ==========================================
    // 3. ÖZELLİKLER (FEATURES)
    // ==========================================
    addBlock('feat-simple-3', '3\'lü Basit', cats.feature, icons.feature, `
        <section style="padding: 80px 20px; background: white;">
            <div style="max-width: 1200px; margin: 0 auto; display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 40px;">
                <div style="text-align: center;">
                    <div style="width: 60px; height: 60px; background: #eff6ff; color: #3b82f6; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 24px; margin: 0 auto 20px;">⚡</div>
                    <h3 style="margin-bottom: 10px;">Yüksek Hız</h3>
                    <p style="color: #666;">Optimize edilmiş altyapı.</p>
                </div>
                <div style="text-align: center;">
                    <div style="width: 60px; height: 60px; background: #eff6ff; color: #3b82f6; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 24px; margin: 0 auto 20px;">🔒</div>
                    <h3 style="margin-bottom: 10px;">Tam Güvenlik</h3>
                    <p style="color: #666;">Verileriniz koruma altında.</p>
                </div>
                <div style="text-align: center;">
                    <div style="width: 60px; height: 60px; background: #eff6ff; color: #3b82f6; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 24px; margin: 0 auto 20px;">💎</div>
                    <h3 style="margin-bottom: 10px;">Premium Kalite</h3>
                    <p style="color: #666;">En iyi standartlar.</p>
                </div>
            </div>
        </section>
    `);

    addBlock('feat-cards', 'Özellik Kartları', cats.feature, icons.feature, `
        <section style="padding: 80px 20px; background: #f8fafc;">
            <div style="max-width: 1200px; margin: 0 auto; display: grid; grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); gap: 30px;">
                <div style="background: white; padding: 30px; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.02); transition: transform 0.2s;">
                    <h3 style="color: #1e293b; margin-bottom: 15px;">Analiz</h3>
                    <p style="color: #64748b;">Detaylı raporlama sistemleri.</p>
                    <a href="#" style="color: #3b82f6; text-decoration: none; font-weight: 500;">İncele &rarr;</a>
                </div>
                <div style="background: white; padding: 30px; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.02); transition: transform 0.2s;">
                    <h3 style="color: #1e293b; margin-bottom: 15px;">Yönetim</h3>
                    <p style="color: #64748b;">Kolay panel kullanımı.</p>
                     <a href="#" style="color: #3b82f6; text-decoration: none; font-weight: 500;">İncele &rarr;</a>
                </div>
                <div style="background: white; padding: 30px; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.02); transition: transform 0.2s;">
                    <h3 style="color: #1e293b; margin-bottom: 15px;">Destek</h3>
                    <p style="color: #64748b;">7/24 Kesintisiz destek.</p>
                     <a href="#" style="color: #3b82f6; text-decoration: none; font-weight: 500;">İncele &rarr;</a>
                </div>
                <div style="background: white; padding: 30px; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.02); transition: transform 0.2s;">
                    <h3 style="color: #1e293b; margin-bottom: 15px;">Bulut</h3>
                    <p style="color: #64748b;">Güçlü bulut altyapısı.</p>
                     <a href="#" style="color: #3b82f6; text-decoration: none; font-weight: 500;">İncele &rarr;</a>
                </div>
            </div>
        </section>
    `);

    addBlock('feat-image-left', 'Resim Solda Özellik', cats.feature, icons.feature, `
        <section style="padding: 60px 20px;">
            <div style="max-width: 1000px; margin: 0 auto; display: flex; align-items: center; gap: 50px; flex-direction: row;">
                <div style="flex:1;"><img src="https://via.placeholder.com/500x350" style="width:100%; border-radius: 12px; box-shadow: 0 10px 20px rgba(0,0,0,0.05);"></div>
                <div style="flex:1;">
                    <h2 style="font-size: 2.5rem; color: #1e293b; margin-bottom: 20px;">Gelişmiş Teknoloji</h2>
                    <p style="color: #64748b; font-size: 1.1rem; line-height: 1.6;">Rakiplerinizin önüne geçmek için en son teknolojileri kullanıyoruz.</p>
                    <ul style="margin-top: 20px; padding-left: 20px; color: #475569;">
                        <li style="margin-bottom: 10px;">Mikroservis Mimarisi</li>
                        <li style="margin-bottom: 10px;">Bulut Tabanlı</li>
                        <li style="margin-bottom: 10px;">Yüksek Ölçeklenebilirlik</li>
                    </ul>
                </div>
            </div>
        </section>
    `);

    addBlock('feat-image-right', 'Resim Sağda Özellik', cats.feature, icons.feature, `
         <section style="padding: 60px 20px; background: #fafafa;">
            <div style="max-width: 1000px; margin: 0 auto; display: flex; align-items: center; gap: 50px; flex-direction: row-reverse;">
                <div style="flex:1;"><img src="https://via.placeholder.com/500x350" style="width:100%; border-radius: 12px; box-shadow: 0 10px 20px rgba(0,0,0,0.05);"></div>
                <div style="flex:1;">
                    <h2 style="font-size: 2.5rem; color: #1e293b; margin-bottom: 20px;">Kullanıcı Dostu</h2>
                    <p style="color: #64748b; font-size: 1.1rem; line-height: 1.6;">Karmaşık süreçleri basitleştiriyoruz.</p>
                    <button style="padding: 10px 20px; border: 1px solid #1e293b; background: transparent; border-radius: 6px; margin-top: 15px;">Daha Fazla</button>
                </div>
            </div>
        </section>
    `);

    // ==========================================
    // 4. İÇERİK (CONTENT)
    // ==========================================
    addBlock('content-basic', 'Temel İçerik', cats.content, icons.content, `<section style="padding: 40px 20px; max-width: 800px; margin: 0 auto; font-size: 1.1rem; line-height: 1.8; color: #333;"><p>Buraya makale veya şirket tanıtım yazısı gelebilir. Okunaklı ve temiz bir tipografi.</p></section>`);
    addBlock('content-quote', 'Alıntı', cats.content, icons.content, `<section style="padding: 60px 20px; text-align: center; background: #fffbeb;"><blockquote style="font-size: 1.8rem; font-style: italic; color: #92400e; max-width: 800px; margin: 0 auto;">"Büyük işler, küçük işlerin birleşimidir."</blockquote><cite style="display: block; margin-top: 20px; color: #b45309; font-weight: bold;">- Anonim</cite></section>`);
    addBlock('content-2col', '2 Kolon Yazı', cats.content, icons.content, `<section style="padding: 60px 20px; max-width: 1000px; margin: 0 auto; display: flex; gap: 40px;"><div style="flex:1;"><h3 style="border-bottom: 2px solid #eee; padding-bottom: 10px;">Vizyon</h3><p>Vizyon metniniz buraya.</p></div><div style="flex:1;"><h3 style="border-bottom: 2px solid #eee; padding-bottom: 10px;">Misyon</h3><p>Misyon metniniz buraya.</p></div></section>`);

    // ==========================================
    // 5. İSTATİSTİKLER (STATS)
    // ==========================================
    addBlock('stats-blue', 'Stats: Mavi', cats.stats, icons.stats, `
        <section style="padding: 60px 20px; background: #1e40af; color: white;">
            <div style="max-width: 1000px; margin: 0 auto; display: flex; justify-content: space-around; text-align: center;">
                <div><div style="font-size: 3rem; font-weight: bold;">500+</div><div>Proje</div></div>
                <div><div style="font-size: 3rem; font-weight: bold;">98%</div><div>Memnuniyet</div></div>
                <div><div style="font-size: 3rem; font-weight: bold;">24/7</div><div>Destek</div></div>
            </div>
        </section>
    `);

    // ==========================================
    // 6. EKİP (TEAM)
    // ==========================================
    addBlock('team-circles', 'Ekip: Yuvarlak', cats.team, icons.team, `
        <section style="padding: 80px 20px; text-align: center;">
            <h2 style="margin-bottom: 50px;">Takımımızla Tanışın</h2>
            <div style="display: flex; justify-content: center; gap: 40px; flex-wrap: wrap;">
                <div><img src="https://via.placeholder.com/150" style="border-radius: 50%; margin-bottom: 15px; width: 120px; height: 120px; object-fit: cover;"><h4>Ali Veli</h4><p style="color: #666;">CEO</p></div>
                <div><img src="https://via.placeholder.com/150" style="border-radius: 50%; margin-bottom: 15px; width: 120px; height: 120px; object-fit: cover;"><h4>Ayşe Yılmaz</h4><p style="color: #666;">CTO</p></div>
                <div><img src="https://via.placeholder.com/150" style="border-radius: 50%; margin-bottom: 15px; width: 120px; height: 120px; object-fit: cover;"><h4>Mehmet Can</h4><p style="color: #666;">Tasarımcı</p></div>
            </div>
        </section>
    `);

    // ==========================================
    // 7. FİYATLANDIRMA (PRICING)
    // ==========================================
    addBlock('price-3col', 'Fiyat: 3 Kart', cats.pricing, icons.pricing, `
        <section style="padding: 80px 20px; background: #f3f4f6;">
            <div style="max-width: 1000px; margin: 0 auto; display: flex; justify-content: center; gap: 20px; align-items: flex-start; flex-wrap: wrap;">
                <div style="background: white; padding: 40px; border-radius: 12px; width: 300px; text-align: center; box-shadow: 0 4px 6px rgba(0,0,0,0.05);">
                    <h3>Başlangıç</h3>
                    <div style="font-size: 2.5rem; font-weight: bold; margin: 20px 0;">₺100</div>
                    <ul style="text-align: left; margin-bottom: 30px; color: #666; list-style: none; padding: 0;"><li>✓ Temel Özellikler</li><li>✓ 1 Kullanıcı</li></ul>
                    <button style="width: 100%; padding: 10px; border: 1px solid #ddd; background: white; border-radius: 6px;">Seç</button>
                </div>
                <div style="background: white; padding: 40px; border-radius: 12px; width: 320px; text-align: center; box-shadow: 0 10px 20px rgba(37, 99, 235, 0.1); border: 2px solid #2563eb; transform: scale(1.05);">
                    <span style="background: #2563eb; color: white; padding: 4px 12px; border-radius: 20px; font-size: 0.8rem; text-transform: uppercase;">Popüler</span>
                    <h3 style="margin-top: 10px;">Pro</h3>
                    <div style="font-size: 2.5rem; font-weight: bold; margin: 20px 0; color: #2563eb;">₺250</div>
                    <ul style="text-align: left; margin-bottom: 30px; color: #666; list-style: none; padding: 0;"><li>✓ Tüm Özellikler</li><li>✓ 5 Kullanıcı</li><li>✓ Öncelikli Destek</li></ul>
                    <button style="width: 100%; padding: 12px; background: #2563eb; color: white; border: none; border-radius: 6px;">Hemen Başla</button>
                </div>
                <div style="background: white; padding: 40px; border-radius: 12px; width: 300px; text-align: center; box-shadow: 0 4px 6px rgba(0,0,0,0.05);">
                    <h3>Kurumsal</h3>
                    <div style="font-size: 2.5rem; font-weight: bold; margin: 20px 0;">Let's Talk</div>
                    <ul style="text-align: left; margin-bottom: 30px; color: #666; list-style: none; padding: 0;"><li>✓ Sınırsız</li><li>✓ Özel Çözümler</li></ul>
                    <button style="width: 100%; padding: 10px; border: 1px solid #ddd; background: white; border-radius: 6px;">İletişim</button>
                </div>
            </div>
        </section>
    `);

    // ==========================================
    // 8. GALERİ & MEDYA
    // ==========================================
    addBlock('gallery-grid', 'Galeri Grid', cats.gallery, icons.gallery, `
        <section style="padding: 60px 20px;">
           <div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(250px, 1fr)); gap: 15px;">
               <img src="https://via.placeholder.com/300" style="width: 100%; height: 250px; object-fit: cover; border-radius: 8px;">
               <img src="https://via.placeholder.com/300" style="width: 100%; height: 250px; object-fit: cover; border-radius: 8px;">
               <img src="https://via.placeholder.com/300" style="width: 100%; height: 250px; object-fit: cover; border-radius: 8px;">
               <img src="https://via.placeholder.com/300" style="width: 100%; height: 250px; object-fit: cover; border-radius: 8px;">
               <img src="https://via.placeholder.com/300" style="width: 100%; height: 250px; object-fit: cover; border-radius: 8px;">
               <img src="https://via.placeholder.com/300" style="width: 100%; height: 250px; object-fit: cover; border-radius: 8px;">
           </div>
        </section>
    `);

    // ==========================================
    // 9. ALT BİLGİ (FOOTER)
    // ==========================================
    addBlock('footer-dark-multi', 'Footer: Koyu Geniş', cats.footer, icons.footer, `
        <footer style="background: #111827; color: #9ca3af; padding: 80px 20px 20px; font-size: 0.9rem;">
            <div style="max-width: 1200px; margin: 0 auto; display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 40px; margin-bottom: 60px;">
                <div>
                    <h4 style="color: white; margin-bottom: 20px; font-size: 1.1rem;">Şirket</h4>
                    <p>Modern çözümler üreten teknoloji şirketi.</p>
                </div>
                <div>
                    <h4 style="color: white; margin-bottom: 20px; font-size: 1.1rem;">Linkler</h4>
                    <ul style="list-style: none; padding: 0;">
                        <li style="margin-bottom: 10px;"><a href="#" style="color: inherit; text-decoration: none;">Anasayfa</a></li>
                        <li style="margin-bottom: 10px;"><a href="#" style="color: inherit; text-decoration: none;">Hakkımızda</a></li>
                        <li style="margin-bottom: 10px;"><a href="#" style="color: inherit; text-decoration: none;">Kariyer</a></li>
                    </ul>
                </div>
                   <div>
                    <h4 style="color: white; margin-bottom: 20px; font-size: 1.1rem;">Yasal</h4>
                    <ul style="list-style: none; padding: 0;">
                        <li style="margin-bottom: 10px;"><a href="#" style="color: inherit; text-decoration: none;">Gizlilik</a></li>
                        <li style="margin-bottom: 10px;"><a href="#" style="color: inherit; text-decoration: none;">Şartlar</a></li>
                    </ul>
                </div>
            </div>
            <div style="border-top: 1px solid #374151; padding-top: 20px; text-align: center;">
                &copy; 2025 Tüm Hakları Saklıdır.
            </div>
        </footer>
    `);

    console.log('✅ Ultimate Blok Paketi (Js) Yüklendi!');


    // ==========================================
    // 10. EKSTRA MODERN HERO BLOKLARI
    // ==========================================
    addBlock('hero-split-diagonal', 'Hero: Çapraz Bölmeli', cats.hero, icons.hero, `
        <section style="position: relative; height: 600px; overflow: hidden; display: flex; align-items: center;">
            <div style="width: 55%; height: 100%; background: #0f172a; color: white; padding: 50px; display: flex; flex-direction: column; justify-content: center; clip-path: polygon(0 0, 100% 0, 85% 100%, 0% 100%); z-index: 2;">
                <h1 style="font-size: 3.5rem; margin-bottom: 20px;">Yenilikçi Tasarım.</h1>
                <p style="font-size: 1.2rem; opacity: 0.8; max-width: 500px;">Sınırları zorlayan görünüm.</p>
                <button style="margin-top: 30px; padding: 15px 30px; background: #3b82f6; color: white; border: none; border-radius: 5px; width: fit-content;">Keşfet</button>
            </div>
            <div style="position: absolute; right: 0; top: 0; width: 60%; height: 100%; background: url('https://images.unsplash.com/photo-1497366216548-37526070297c?auto=format&fit=crop&w=1600&q=80') center/cover; z-index: 1;"></div>
        </section>
    `);

    addBlock('hero-overlay-text', 'Hero: Metin Üzeri', cats.hero, icons.hero, `
        <section style="background: url('https://images.unsplash.com/photo-1519389950473-47ba0277781c?auto=format&fit=crop&w=1600&q=80') center/cover; height: 500px; display: flex; align-items: center; justify-content: center; position: relative;">
            <div style="position: absolute; top:0; left:0; width:100%; height:100%; background: rgba(0,0,0,0.6);"></div>
            <div style="background: rgba(255,255,255,0.1); backdrop-filter: blur(10px); padding: 50px; border-radius: 20px; text-align: center; color: white; border: 1px solid rgba(255,255,255,0.2); z-index: 2; max-width: 700px;">
                <h1 style="font-size: 3rem; text-shadow: 0 4px 10px rgba(0,0,0,0.3);">Modern Cam Efekti</h1>
                <p style="font-size: 1.2rem;">Camgöbeği tasarımı ile estetik duruş.</p>
            </div>
        </section>
    `);

    addBlock('hero-slider-static', 'Hero: Statik Slider', cats.hero, icons.hero, `
        <section style="display: flex; height: 500px;">
            <div style="flex: 1; background: url('https://images.unsplash.com/photo-1522202176988-66273c2fd55f?auto=format&fit=crop&w=800&q=80') center/cover; position: relative;"><div style="position:absolute; bottom:20px; left:20px; color:white; font-weight:bold; font-size:1.5rem; text-shadow:0 2px 4px rgba(0,0,0,0.5);">Toplantı</div></div>
            <div style="flex: 1; background: url('https://images.unsplash.com/photo-1556761175-5973dc0f32e7?auto=format&fit=crop&w=800&q=80') center/cover; position: relative;"><div style="position:absolute; bottom:20px; left:20px; color:white; font-weight:bold; font-size:1.5rem; text-shadow:0 2px 4px rgba(0,0,0,0.5);">İş Birliği</div></div>
            <div style="flex: 1; background: url('https://images.unsplash.com/photo-1600880292203-757bb62b4baf?auto=format&fit=crop&w=800&q=80') center/cover; position: relative;"><div style="position:absolute; bottom:20px; left:20px; color:white; font-weight:bold; font-size:1.5rem; text-shadow:0 2px 4px rgba(0,0,0,0.5);">Başarı</div></div>
        </section>
    `);

    addBlock('hero-video-popup', 'Hero: Video Popup', cats.hero, icons.hero, `
        <section style="padding: 100px 20px; background: white; text-align: center; max-width: 1000px; margin: auto;">
            <div style="position: relative; display: inline-block;">
                <img src="https://images.unsplash.com/photo-1516321318423-f06f85e504b3?auto=format&fit=crop&w=1000&q=80" style="width: 100%; border-radius: 20px; filter: brightness(0.7);">
                <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); width: 80px; height: 80px; background: white; border-radius: 50%; display: flex; align-items: center; justify-content: center; cursor: pointer; box-shadow: 0 0 0 10px rgba(255,255,255,0.3);">
                    <svg width="30" height="30" viewBox="0 0 24 24" fill="#000"><path d="M8 5v14l11-7z"/></svg>
                </div>
            </div>
            <h2 style="margin-top: 30px;">Hikayemizi İzleyin</h2>
        </section>
    `);


    // ==========================================
    // 11. GELİŞMİŞ İÇERİK BLOKLARI
    // ==========================================
    addBlock('content-timeline', 'İçerik: Zaman Tüneli', cats.content, icons.content, `
        <section style="padding: 60px 20px; max-width: 800px; margin: auto; position: relative; border-left: 2px solid #e2e8f0; padding-left: 40px;">
            <div style="margin-bottom: 40px; position: relative;">
                <div style="position: absolute; left: -49px; top: 0; width: 16px; height: 16px; background: #3b82f6; border-radius: 50%; border: 4px solid white; box-shadow: 0 0 0 1px #e2e8f0;"></div>
                <span style="color: #64748b; font-size: 0.9rem;">2023 - Q1</span>
                <h3 style="margin: 5px 0;">Kuruluş</h3>
                <p>Şirketimizin temelleri atıldı.</p>
            </div>
             <div style="margin-bottom: 40px; position: relative;">
                <div style="position: absolute; left: -49px; top: 0; width: 16px; height: 16px; background: #3b82f6; border-radius: 50%; border: 4px solid white; box-shadow: 0 0 0 1px #e2e8f0;"></div>
                <span style="color: #64748b; font-size: 0.9rem;">2024 - Q2</span>
                <h3 style="margin: 5px 0;">Büyüme</h3>
                <p>İlk 1000 müşteriye ulaştık.</p>
            </div>
             <div style="position: relative;">
                <div style="position: absolute; left: -49px; top: 0; width: 16px; height: 16px; background: #10b981; border-radius: 50%; border: 4px solid white; box-shadow: 0 0 0 1px #e2e8f0;"></div>
                <span style="color: #64748b; font-size: 0.9rem;">2025 - Hedef</span>
                <h3 style="margin: 5px 0;">Globalleşme</h3>
                <p>Dünya pazarına açılıyoruz.</p>
            </div>
        </section>
    `);

    addBlock('content-numbers', 'İçerik: Rakamlarla', cats.content, icons.content, `
        <section style="padding: 80px 20px; background: #111; color: white;">
            <div style="max-width: 1200px; margin: auto; display: grid; grid-template-columns: 1fr 1fr; gap: 50px; align-items: center;">
                <div>
                    <h2 style="font-size: 3rem;">Rakamlarla Biz</h2>
                    <p style="opacity: 0.7; font-size: 1.2rem;">Başarımızı kanıtlayan veriler.</p>
                </div>
                <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 30px;">
                    <div style="background: #222; padding: 30px; border-radius: 12px;"><h3>10+</h3><p>Yıl</p></div>
                    <div style="background: #222; padding: 30px; border-radius: 12px;"><h3>500+</h3><p>Müşteri</p></div>
                    <div style="background: #222; padding: 30px; border-radius: 12px;"><h3>50+</h3><p>Ödül</p></div>
                    <div style="background: #222; padding: 30px; border-radius: 12px;"><h3>%99</h3><p>Uptime</p></div>
                </div>
            </div>
        </section>
    `);

    addBlock('content-check-list', 'İçerik: Kontrol Listesi', cats.content, icons.content, `
        <section style="padding: 60px 20px; max-width: 800px; margin: auto;">
            <h2 style="text-align: center; margin-bottom: 40px;">Paket İçeriği</h2>
            <ul style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px; list-style: none; padding: 0;">
                <li style="background: #f1f5f9; padding: 15px; border-radius: 8px;">✅ Responsive Tasarım</li>
                <li style="background: #f1f5f9; padding: 15px; border-radius: 8px;">✅ SEO Uyumlu</li>
                <li style="background: #f1f5f9; padding: 15px; border-radius: 8px;">✅ Sınırsız Renk</li>
                <li style="background: #f1f5f9; padding: 15px; border-radius: 8px;">✅ 7/24 Teknik Destek</li>
                <li style="background: #f1f5f9; padding: 15px; border-radius: 8px;">✅ Ücretsiz Güncelleme</li>
                <li style="background: #f1f5f9; padding: 15px; border-radius: 8px;">✅ Kolay Kurulum</li>
            </ul>
        </section>
    `);

    addBlock('content-author', 'İçerik: Yazar', cats.content, icons.content, `
        <section style="padding: 40px; border: 1px solid #eee; border-radius: 12px; margin: 40px auto; max-width: 700px; display: flex; align-items: center; gap: 30px;">
            <img src="https://via.placeholder.com/100" style="border-radius: 50%; width: 100px; height: 100px; object-fit: cover;">
            <div>
                <h4 style="margin: 0 0 10px 0;">Yazar Hakkında</h4>
                <p style="margin: 0; color: #666;">Teknoloji ve yazılım dünyasında 10 yıllık deneyime sahip içerik üreticisi. Modern web trendlerini takip etmeyi sever.</p>
                <div style="margin-top: 10px; font-weight: bold; color: #2563eb;">Ahmet Yazar</div>
            </div>
        </section>
    `);

    addBlock('content-accordion', 'İçerik: Akordeon', cats.content, icons.content, `
       <section style="max-width: 800px; margin: 50px auto; padding: 20px;">
           <details open style="margin-bottom: 15px; padding: 15px; border: 1px solid #ddd; border-radius: 8px;">
               <summary style="font-weight: bold; cursor: pointer;">Proje ne kadar sürer?</summary>
               <p style="margin-top: 10px; color: #555;">Ortalama proje süreci kapsamına göre 2-4 hafta arasında değişmektedir.</p>
           </details>
           <details style="margin-bottom: 15px; padding: 15px; border: 1px solid #ddd; border-radius: 8px;">
               <summary style="font-weight: bold; cursor: pointer;">Ödeme seçenekleri nedir?</summary>
               <p style="margin-top: 10px; color: #555;">Kredi kartı, havale ve taksitli ödeme seçeneklerimiz mevcuttur.</p>
           </details>
       </section>
    `);


    // ==========================================
    // 12. YENİ CTA VE BUTON GRUPLARI
    // ==========================================
    addBlock('cta-app-download', 'CTA: Uygulama İndir', cats.cta, icons.cta, `
        <section style="padding: 80px 20px; background: linear-gradient(to right, #2563eb, #4f46e5); color: white; text-align: center;">
            <h2 style="font-size: 2.5rem; margin-bottom: 30px;">Uygulamamızı İndirin</h2>
            <div style="display: flex; justify-content: center; gap: 20px;">
                <button style="background: black; color: white; padding: 12px 25px; border-radius: 8px; border: 1px solid rgba(255,255,255,0.2); display: flex; align-items: center; gap: 10px;">
                    <span style="font-size: 20px;">🍎</span> <span>App Store</span>
                </button>
                <button style="background: black; color: white; padding: 12px 25px; border-radius: 8px; border: 1px solid rgba(255,255,255,0.2); display: flex; align-items: center; gap: 10px;">
                    <span style="font-size: 20px;">▶️</span> <span>Play Store</span>
                </button>
            </div>
        </section>
    `);

    addBlock('cta-subscribe-minimal', 'CTA: Minimal Abone', cats.cta, icons.cta, `
        <section style="border-top: 1px solid #eee; border-bottom: 1px solid #eee; padding: 50px 20px; text-align: center;">
            <p style="color: #666; margin-bottom: 20px; text-transform: uppercase; letter-spacing: 2px; font-size: 0.9rem;">Gelişmeleri Takip Edin</p>
            <div style="display: inline-flex; max-width: 400px; width: 100%;">
                <input type="email" placeholder="E-posta adresiniz" style="flex:1; padding: 12px; border: 1px solid #ddd; border-right: none; border-radius: 4px 0 0 4px;">
                <button style="background: #111; color: white; border: none; padding: 0 25px; border-radius: 0 4px 4px 0;">Gönder</button>
            </div>
        </section>
    `);

    addBlock('cta-two-buttons', 'CTA: İkili Buton', cats.cta, icons.cta, `
        <div style="text-align: center; padding: 40px;">
            <button style="padding: 12px 30px; background: #2563eb; color: white; border: none; border-radius: 6px; margin: 10px;">Şimdi Satın Al</button>
            <button style="padding: 12px 30px; background: white; color: #2563eb; border: 1px solid #2563eb; border-radius: 6px; margin: 10px;">Demoyu İncele</button>
        </div>
    `);

    addBlock('cta-banner-text', 'CTA: Basit Banner', cats.cta, icons.cta, `
        <section style="background: #fbbf24; padding: 15px; text-align: center; font-weight: bold; color: #78350f;">
            🚀 Sınırlı süre için %50 indirim! <a href="#" style="color: black; text-decoration: underline; margin-left: 10px;">Detayları Gör</a>
        </section>
    `);


    // ==========================================
    // 13. REFERANSLAR VE YORUMLAR (TESTIMONIALS)
    // ==========================================
    addBlock('testimonial-single', 'Referans: Tekli', cats.testimonial, icons.content, `
        <section style="padding: 80px 20px; text-align: center; max-width: 800px; margin: auto;">
             <div style="font-size: 3rem; color: #cbd5e1; line-height: 1;">“</div>
             <p style="font-size: 1.5rem; color: #334155; margin: 20px 0;">Bu ürün iş akışımızı tamamen değiştirdi. Artık her şey çok daha hızlı ve güvenilir.</p>
             <div style="display: flex; align-items: center; justify-content: center; gap: 15px; margin-top: 30px;">
                 <img src="https://via.placeholder.com/60" style="border-radius: 50%;">
                 <div style="text-align: left;">
                     <strong>Elif Yurtseven</strong>
                     <div style="color: #64748b; font-size: 0.9rem;">Pazarlama Müdürü</div>
                 </div>
             </div>
        </section>
    `);

    addBlock('testimonial-grid', 'Referans: Grid', cats.testimonial, icons.content, `
        <section style="padding: 60px 20px; background: #f8fafc;">
            <div style="max-width: 1200px; margin: auto; display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 30px;">
                <div style="background: white; padding: 30px; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.02);">
                    <div style="color: #fbbf24; margin-bottom: 10px;">★★★★★</div>
                    <p>"Harika bir deneyim!"</p>
                    <small>— Müşteri A</small>
                </div>
                <div style="background: white; padding: 30px; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.02);">
                    <div style="color: #fbbf24; margin-bottom: 10px;">★★★★★</div>
                    <p>"Kesinlikle tavsiye ederim."</p>
                    <small>— Müşteri B</small>
                </div>
                <div style="background: white; padding: 30px; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.02);">
                    <div style="color: #fbbf24; margin-bottom: 10px;">★★★★★</div>
                    <p>"Destek ekibi çok ilgili."</p>
                    <small>— Müşteri C</small>
                </div>
            </div>
        </section>
    `);

    addBlock('logo-cloud', 'Logo Bulutu', cats.testimonial, icons.content, `
        <section style="padding: 50px 20px; text-align: center;">
            <p style="color: #94a3b8; font-weight: 500; margin-bottom: 30px;">BİZE GÜVENEN 500+ ŞİRKET</p>
            <div style="display: flex; justify-content: center; gap: 40px; flex-wrap: wrap; opacity: 0.6; filter: grayscale(100%);">
                <div style="font-size: 24px; font-weight: bold;">GOOGLE</div>
                <div style="font-size: 24px; font-weight: bold;">MICROSOFT</div>
                <div style="font-size: 24px; font-weight: bold;">AMAZON</div>
                <div style="font-size: 24px; font-weight: bold;">NETFLIX</div>
                <div style="font-size: 24px; font-weight: bold;">TESLA</div>
            </div>
        </section>
    `);


    // ==========================================
    // 14. İLETİŞİM VE FORMLAR
    // ==========================================
    addBlock('contact-simple', 'İletişim: Basit', cats.form, icons.form, `
        <section style="padding: 60px 20px; max-width: 600px; margin: auto;">
            <h2 style="text-align: center; margin-bottom: 30px;">Bize Ulaşın</h2>
            <form style="display: grid; gap: 15px;">
                <input type="text" placeholder="Adınız" style="padding: 12px; border: 1px solid #ddd; border-radius: 6px;">
                <input type="email" placeholder="E-posta" style="padding: 12px; border: 1px solid #ddd; border-radius: 6px;">
                <textarea rows="4" placeholder="Mesajınız" style="padding: 12px; border: 1px solid #ddd; border-radius: 6px;"></textarea>
                <button style="padding: 12px; background: #2563eb; color: white; border: none; border-radius: 6px; font-weight: bold;">Gönder</button>
            </form>
        </section>
    `);

    addBlock('contact-split', 'İletişim: Bölmeli', cats.form, icons.form, `
        <section style="display: grid; grid-template-columns: 1fr 1fr; max-width: 1000px; margin: 50px auto; border-radius: 12px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.1);">
            <div style="background: #1e293b; color: white; padding: 40px;">
                <h3>İletişim Bilgileri</h3>
                <p>Sorularınız mı var? Bize her zaman ulaşabilirsiniz.</p>
                <div style="margin-top: 30px;">
                    <p>📍 İstanbul, Türkiye</p>
                    <p>📧 info@ydeveloper.com</p>
                    <p>📞 +90 555 555 55 55</p>
                </div>
            </div>
            <div style="background: white; padding: 40px;">
                <form style="display: grid; gap: 15px;">
                     <input type="text" placeholder="Ad Soyad" style="padding: 10px; border: 1px solid #eee; border-radius: 4px;">
                     <input type="email" placeholder="Email" style="padding: 10px; border: 1px solid #eee; border-radius: 4px;">
                     <button style="background: #1e293b; color: white; padding: 10px; border: none; border-radius: 4px;">Gönder</button>
                </form>
            </div>
        </section>
    `);

    addBlock('newsletter-card', 'Bülten Kartı', cats.form, icons.form, `
        <div style="background: #f0f9ff; border: 1px solid #bae6fd; padding: 30px; border-radius: 12px; text-align: center; max-width: 500px; margin: 40px auto;">
            <h3 style="color: #0369a1;">Haftalık Bülten</h3>
            <p style="color: #0c4a6e; margin-bottom: 20px;">Teknoloji dünyasından en son haberler her hafta kutunuzda.</p>
            <div style="display: flex; gap: 10px;">
                <input type="email" placeholder="Email adresi" style="flex:1; padding: 10px; border: 1px solid #bae6fd; border-radius: 6px;">
                <button style="background: #0ea5e9; color: white; border: none; padding: 10px 20px; border-radius: 6px;">Abone Ol</button>
            </div>
        </div>
    `);


    // ==========================================
    // 15. KARIŞIK EKSTRA BLOKLAR (Footer, Navigation vs.)
    // ==========================================
    addBlock('footer-minimal', 'Footer: Minimal Merkez', cats.footer, icons.footer, `
        <footer style="text-align: center; padding: 40px 20px;">
             <div style="font-weight: bold; font-size: 1.5rem; margin-bottom: 20px;">LOGO</div>
             <div style="display: flex; justify-content: center; gap: 30px; margin-bottom: 30px; color: #666;">
                 <a href="#" style="color: inherit; text-decoration: none;">Ürünler</a>
                 <a href="#" style="color: inherit; text-decoration: none;">Şirket</a>
                 <a href="#" style="color: inherit; text-decoration: none;">Blog</a>
             </div>
             <div style="color: #999; font-size: 0.9rem;">© 2025 Company Inc.</div>
        </footer>
    `);

    addBlock('footer-newsletter', 'Footer: Bültenli', cats.footer, icons.footer, `
        <footer style="background: #111; color: white; padding: 60px 20px;">
            <div style="max-width: 1200px; margin: auto; display: flex; flex-wrap: wrap; justify-content: space-between; align-items: center; gap: 40px;">
                <div>
                    <h3>Haber Bülteni</h3>
                    <p style="color: #888;">Güncellemelerden haberdar olun.</p>
                </div>
                <div style="display: flex; gap: 10px;">
                    <input type="text" placeholder="Email" style="padding: 10px; border-radius: 4px; border: none;">
                    <button style="padding: 10px 20px; background: #3b82f6; color: white; border: none; border-radius: 4px;">Kayıt</button>
                </div>
            </div>
            <div style="border-top: 1px solid #333; margin-top: 40px; padding-top: 20px; text-align: center; color: #666;">
                Telif Hakkı Saklıdır.
            </div>
        </footer>
    `);

    addBlock('nav-simple', 'Nav: Basit', cats.layout, icons.layout, `
        <nav style="display: flex; justify-content: space-between; align-items: center; padding: 20px 40px; border-bottom: 1px solid #eee;">
            <div style="font-weight: bold; font-size: 1.2rem;">MARKAM</div>
            <div style="display: flex; gap: 20px;">
                <a href="#" style="text-decoration: none; color: #333;">Anasayfa</a>
                <a href="#" style="text-decoration: none; color: #333;">Hakkımızda</a>
                <a href="#" style="text-decoration: none; color: #333;">İletişim</a>
            </div>
            <button style="padding: 8px 16px; background: black; color: white; border-radius: 4px; text-decoration: none;">Giriş</button>
        </nav>
    `);

    addBlock('card-profile', 'Kart: Profil', cats.team, icons.team, `
        <div style="background: white; border: 1px solid #eee; padding: 20px; border-radius: 10px; width: 300px; margin: auto; text-align: center;">
            <div style="width: 80px; height: 80px; background: #eee; border-radius: 50%; margin: 0 auto 15px;"></div>
            <h3>Kullanıcı Adı</h3>
            <p style="color: #666; font-size: 0.9rem;">Yazılım Mühendisi</p>
            <button style="margin-top: 15px; width: 100%; padding: 8px; border: 1px solid #3b82f6; background: white; color: #3b82f6; border-radius: 4px;">Takip Et</button>
        </div>
    `);

    addBlock('card-product', 'Kart: Ürün', cats.content, icons.content, `
        <div style="border: 1px solid #eee; border-radius: 12px; overflow: hidden; max-width: 300px; margin: auto;">
            <div style="height: 200px; background: #f8fafc; display: flex; align-items: center; justify-content: center;">Ürün Görseli</div>
            <div style="padding: 20px;">
                <h3 style="margin: 0 0 10px 0;">Premium Kulaklık</h3>
                <div style="font-size: 1.2rem; font-weight: bold; color: #2563eb; margin-bottom: 15px;">₺2,500</div>
                <button style="width: 100%; padding: 10px; background: #2563eb; color: white; border: none; border-radius: 6px;">Sepete Ekle</button>
            </div>
        </div>
    `);

    addBlock('grid-gallery-masonry', 'Galeri: Masonry', cats.gallery, icons.gallery, `
        <div style="columns: 3 200px; gap: 20px;">
            <div style="margin-bottom: 20px;"><img src="https://via.placeholder.com/200x300" style="width:100%; border-radius: 8px;"></div>
            <div style="margin-bottom: 20px;"><img src="https://via.placeholder.com/200x150" style="width:100%; border-radius: 8px;"></div>
            <div style="margin-bottom: 20px;"><img src="https://via.placeholder.com/200x400" style="width:100%; border-radius: 8px;"></div>
            <div style="margin-bottom: 20px;"><img src="https://via.placeholder.com/200x250" style="width:100%; border-radius: 8px;"></div>
            <div style="margin-bottom: 20px;"><img src="https://via.placeholder.com/200x200" style="width:100%; border-radius: 8px;"></div>
        </div>
    `);

    addBlock('progress-bars', 'İlerleme Çubukları', cats.content, icons.content, `
        <div style="max-width: 600px; margin: auto; padding: 20px;">
            <div style="margin-bottom: 15px;">
                <div style="display:flex; justify-content:space-between; margin-bottom:5px;"><span>Tasarım</span><span>90%</span></div>
                <div style="background:#eee; height:10px; border-radius:5px;"><div style="background:#2563eb; width:90%; height:100%; border-radius:5px;"></div></div>
            </div>
            <div style="margin-bottom: 15px;">
                <div style="display:flex; justify-content:space-between; margin-bottom:5px;"><span>Yazılım</span><span>85%</span></div>
                <div style="background:#eee; height:10px; border-radius:5px;"><div style="background:#ec4899; width:85%; height:100%; border-radius:5px;"></div></div>
            </div>
             <div>
                <div style="display:flex; justify-content:space-between; margin-bottom:5px;"><span>Pazarlama</span><span>70%</span></div>
                <div style="background:#eee; height:10px; border-radius:5px;"><div style="background:#f59e0b; width:70%; height:100%; border-radius:5px;"></div></div>
            </div>
        </div>
    `);

    addBlock('alert-box', 'Uyarı Kutuları', cats.content, icons.content, `
        <div style="max-width: 600px; margin: auto; padding: 20px; display: grid; gap: 15px;">
            <div style="background: #ecfdf5; color: #065f46; padding: 15px; border-radius: 6px; border: 1px solid #a7f3d0;">✅ İşlem başarıyla tamamlandı.</div>
            <div style="background: #fffbeb; color: #92400e; padding: 15px; border-radius: 6px; border: 1px solid #fcd34d;">⚠️ Lütfen bilgilerinizi kontrol edin.</div>
            <div style="background: #fef2f2; color: #991b1b; padding: 15px; border-radius: 6px; border: 1px solid #fecaca;">❌ Bir hata oluştu, tekrar deneyin.</div>
        </div>
    `);

    // ... (Daha fazla blok eklenebilir)
}
